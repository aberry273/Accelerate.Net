using Accelerate.Features.Account.Controllers;
using Accelerate.Features.Account.Models.Views;
using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Helpers;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.UI.Components.Table;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Media.Models.Data;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Aggregations;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Security.Policy;
using System.Threading.Channels;
using static System.Net.WebRequestMethods;

namespace Accelerate.Features.Account.Services
{
    public class AccountViewService : IAccountViewService
    {
        private readonly SignInManager<AccountUser> _signInManager;
        private OAuthConfiguration _OAuthConfig;
        private IMetaContentService _contentService;
        public AccountViewService(
            SignInManager<AccountUser> signInManager,
            IMetaContentService contentService,
            IOptions<OAuthConfiguration> options)
        {
            _signInManager = signInManager;
            _contentService = contentService;
            _OAuthConfig = options.Value;
        }
        #region Notifications

        public ManagePage GetNotificationsPage(AccountUser user, IEnumerable<ContentPostActivityEntity> activities, int totalActivities)
        {
            var viewModel = new MentionPage(GetManagePage(user));
            viewModel.Table = this.GetMentionsTable();
            viewModel.Table.Pages =(totalActivities / activities.Count());
            viewModel.Table.ItemsPerPage = activities.Count();
            viewModel.Table.Items = activities.Select(CreateRow).ToList();
            viewModel.Table.PostbackUrl = "/api/ContentPostActivity/query";
            return viewModel;
        }

        List<string> CreateRow(ContentPostActivityEntity activity)
        {
            return new List<string>()
            {
                activity.CreatedOn.ToLongDateString(),
                activity.SourceId.ToString(),
                activity.UserId.ToString()
            };
        }

        #endregion
        #region Mentions

        public ManagePage GetMentionsPage(AccountUser user)
        {
            var viewModel = new MentionPage(GetManagePage(user));
            viewModel.Table = this.GetMentionsTable();
            return viewModel;
        }

        private AjaxAclTable<string> GetMentionsTable()
        {
            var model = new AjaxAclTable<string>();
            var headers = new List<AclTableHeader>()
            {
                new AclTableHeader()
                {
                    Text = "createdOn",
                    Label = "Date",
                    Type = AclTableHeaderType.Date
                },
                new AclTableHeader()
                {
                    Text = "message",
                    Label = "Message",
                },
                new AclTableHeader()
                {
                    Text = "url",
                    Label = "Url",
                    Data = new { Type = "Icon", Icon = "home" },
                    Type = AclTableHeaderType.Link
                }
            }; 
            model.Headers = headers;
            return model;
        }

        #endregion
        #region Manage

        public ManagePage GetManagePage(AccountUser user)
        {
            var profile = Accelerate.Foundations.Account.Helpers.AccountHelpers.CreateUserProfile(user);

            var viewModel = new ManagePage(_contentService.CreatePageBaseContent(profile));
            viewModel.UserId = user.Id;
            viewModel.UserStatus = user.Status;
            viewModel.ProfileImageForm = CreateProfileImageForm(user);
            viewModel.UserForm = CreateUserForm(user);
            viewModel.ProfileForm = CreateProfileForm(user);
            viewModel.DeactivateForm = CreateDeactivateForm(user);
            viewModel.DeleteForm = CreateDeleteForm(user);
            viewModel.ReactivateForm = CreateReactivateForm(user);
            viewModel.ModalCreateMedia = CreateModalMediaForm(user);
            viewModel.FilterEvent = "onFilterChange";
            viewModel.Tabs = new List<NavigationItem>()
            {
                GetPageLink(nameof(AccountController.Profile)),
                GetPageLink(nameof(AccountController.Posts)),
                //GetPageLink(nameof(AccountController.Mentions)),
                GetPageLink(nameof(AccountController.Media)),
                GetPageLink(nameof(AccountController.Notifications)),
                //GetPageLink(nameof(AccountController.Settings)),
            };
            return viewModel;
        }


        public List<NavigationFilterValue> GetFilterSortOptions()
        {
            return new List<NavigationFilterValue>()
            {
                new NavigationFilterValue()
                {
                    Key = Foundations.Content.Constants.Fields.CreatedOn,
                    Name = "Created"
                },
                new NavigationFilterValue()
                {
                    Key = Foundations.Content.Constants.Fields.UpdatedOn,
                    Name = "Updated"
                },
                new NavigationFilterValue()
                {
                    Key = Foundations.Content.Constants.Fields.Replies,
                    Name = "Replies"
                },
                new NavigationFilterValue()
                {
                    Key = Foundations.Content.Constants.Fields.Quotes,
                    Name = "Quotes"
                },
                new NavigationFilterValue()
                {
                    Key = Foundations.Content.Constants.Fields.TotalVotes,
                    Name = "Total Votes"
                },
            };
        }
        public List<NavigationFilterValue> GetFilterSortOrderOptions()
        {
            return new List<NavigationFilterValue>()
            {
                new NavigationFilterValue()
                {
                    Key = "Asc",
                    Name = "Asc"
                },
                new NavigationFilterValue()
                {
                    Key = "Desc",
                    Name = "Desc"
                },
            };
        }
        #region MediaFilters
        public NavigationFilter CreateMediaNavigationFilters(SearchResponse<MediaBlobDocument> aggregateResponse)
        {
            return new NavigationFilter()
            {
                Filters = CreateMediaSearchFilters(aggregateResponse),
                Sort = new NavigationFilterItem()
                {
                    Name = Constants.Filters.Sort,
                    FilterType = NavigationFilterType.Select,
                    Values = GetFilterSortOptions()
                },
                SortBy = new NavigationFilterItem()
                {
                    Name = Constants.Filters.SortOrder,
                    FilterType = NavigationFilterType.Select,
                    Values = GetFilterSortOrderOptions()
                }
            };
        }
        public Dictionary<string, string> GetMediaFilterOptions()
        {
            return new Dictionary<string, string>()
            {
                {
                    Constants.Filters.Media.Tags,
                    Foundations.Media.Constants.Fields.Tags
                },
                {
                    Constants.Filters.Media.Type,
                    Foundations.Media.Constants.Fields.Type
                },
            };
        }
        public List<NavigationFilterItem> CreateMediaSearchFilters(SearchResponse<MediaBlobDocument> aggregateResponse)
        {
            var filterValues = new Dictionary<string, List<NavigationFilterValue>>();
            if (aggregateResponse.IsValidResponse)
            {
                var filterOptions = GetMediaFilterOptions();
                filterValues = filterOptions.Values.ToDictionary(x => x, x => GetValuesFromAggregate(aggregateResponse.Aggregations, x));
            }
            return CreateMediaNavigationFilters(filterValues);
        }
        private List<NavigationFilterItem> CreateMediaNavigationFilters(IDictionary<string, List<NavigationFilterValue>> filters)
        {
            if (filters == null) filters = new Dictionary<string, List<NavigationFilterValue>>();
            var filter = new List<NavigationFilterItem>();
            //TODO 
            var Actions = GetAggregateValues(filters, GetFilterKey(Constants.Filters.Media.Tags));
            if (Actions.Count > 0)
            {
                filter.Add(new NavigationFilterItem()
                {
                    Name = Constants.Filters.Media.Tags,
                    FilterType = NavigationFilterType.Checkbox,
                    Values = Actions
                });
            }
            var threads = GetAggregateValues(filters, GetFilterKey(Constants.Filters.Media.Type));
            if (threads.Count > 0)
            {
                filter.Add(new NavigationFilterItem()
                {
                    Name = Constants.Filters.Media.Type,
                    FilterType = NavigationFilterType.Select,
                    Values = threads
                });
            } 
            return filter;
        }
        #endregion

        #region Post Filters
        public NavigationFilter CreatePostNavigationFilters(SearchResponse<ContentPostDocument> aggregateResponse)
        {
            return new NavigationFilter()
            {
                Filters = CreatePostSearchFilters(aggregateResponse),
                Sort = new NavigationFilterItem()
                {
                    Name = Constants.Filters.Sort,
                    FilterType = NavigationFilterType.Select,
                    Values = GetFilterSortOptions()
                },
                SortBy = new NavigationFilterItem()
                {
                    Name = Constants.Filters.SortOrder,
                    FilterType = NavigationFilterType.Select,
                    Values = GetFilterSortOrderOptions()
                }
            };
        }
        public Dictionary<string, string> GetPostFilterOptions()
        {
            return new Dictionary<string, string>()
            {
                {
                    Constants.Filters.Posts.Tags,
                    Foundations.Content.Constants.Fields.Tags
                },
                {
                    Constants.Filters.Posts.Threads,
                    Foundations.Content.Constants.Fields.ShortThreadId
                },
                {
                    Constants.Filters.Posts.Quotes,
                    Foundations.Content.Constants.Fields.QuoteIds
                }
            };
        }
        // FILTERS - Posts
        public List<NavigationFilterItem> CreatePostSearchFilters(SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var filterValues = new Dictionary<string, List<NavigationFilterValue>>();
            if (aggregateResponse.IsValidResponse)
            {
                var filterOptions = GetPostFilterOptions();
                filterValues = filterOptions.Values.ToDictionary(x => x, x => GetValuesFromAggregate(aggregateResponse.Aggregations, x));
            }
            return CreatePostNavigationFilters(filterValues);
        }
        private List<NavigationFilterValue> GetValuesFromAggregate(AggregateDictionary aggregates, string key)
        {
            var agg = aggregates.FirstOrDefault(x => x.Key == key);
            StringTermsAggregate vals = agg.Value as StringTermsAggregate;
            if (vals == null || vals.Buckets == null || vals.Buckets.Count == 0) return new List<NavigationFilterValue>();

            var results = vals.Buckets
                .Where(x => !string.IsNullOrEmpty(x.Key.Value.ToString()))
                .Select(x => new NavigationFilterValue()
                {

                    Key = x.Key.Value.ToString(),
                    Name = x.Key.Value.ToString(),
                    Count = x.DocCount
                }).
                ToList();
            return results;
        }
        public List<QueryFilter> GetActualFilterKeys(List<QueryFilter>? Filters)
        {
            return Filters?.Select(x =>
            {
                x.Name = GetFilterKey(x.Name);
                return x;
            }).ToList();
        }
        public string GetFilterKey(string key)
        {
            var keyVal = this.GetPostFilterOptions().FirstOrDefault(x => x.Key == key);
            if (keyVal.Value == null) return key.ToCamelCase();
            return keyVal.Value?.ToCamelCase();
        }
        private List<NavigationFilterValue> GetAggregateValues(IDictionary<string, List<NavigationFilterValue>> aggFilters, string key)
        {
            if (key == null) return new List<NavigationFilterValue>();
            return aggFilters.ContainsKey(key) ? aggFilters[key] : new List<NavigationFilterValue>();
        }
        private List<NavigationFilterItem> CreatePostNavigationFilters(IDictionary<string, List<NavigationFilterValue>> filters)
        {
            if (filters == null) filters = new Dictionary<string, List<NavigationFilterValue>>();
            var filter = new List<NavigationFilterItem>();

            var Actions = GetAggregateValues(filters, GetFilterKey(Constants.Filters.Posts.Actions));
            if (Actions.Count > 0)
            {
                filter.Add(new NavigationFilterItem()
                {
                    Name = Constants.Filters.Posts.Actions,
                    FilterType = NavigationFilterType.Select,
                    Values = Actions
                });
            }
            var threads = GetAggregateValues(filters, GetFilterKey(Constants.Filters.Posts.Threads));
            if (threads.Count > 0)
            {
                filter.Add(new NavigationFilterItem()
                {
                    Name = Constants.Filters.Posts.Threads,
                    FilterType = NavigationFilterType.Checkbox,
                    Values = threads
                });
            }
            var quotes = new List<NavigationFilterValue>();// GetAggregateValues(filters, GetFilterKey(Constants.Filters.Posts.Quotes));
            if (quotes.Count > 0)
            {
                filter.Add(new NavigationFilterItem()
                {
                    Name = Constants.Filters.Posts.Quotes,
                    FilterType = NavigationFilterType.Checkbox,
                    Values = quotes
                });
            }

            var tags = new List<NavigationFilterValue>();// GetAggregateValues(filters, GetFilterKey(Constants.Filters.Posts.Tags));
            if (tags.Count > 0)
            {
                filter.Add(new NavigationFilterItem()
                {
                    Name = Constants.Filters.Posts.Tags,
                    FilterType = NavigationFilterType.Checkbox,
                    Values = tags
                });
            }

            var content = new List<NavigationFilterValue>();//  GetAggregateValues(filters, GetFilterKey(Constants.Filters.Posts.Content));
            if (content.Count > 0)
            {
                filter.Add(new NavigationFilterItem()
                {
                    Name = Constants.Filters.Posts.Content,
                    FilterType = NavigationFilterType.Select,
                    Values = content
                });
            }

            var sort = new List<NavigationFilterValue>();// GetAggregateValues(filters, GetFilterKey(Constants.Filters.Posts.Sort));
            if (sort.Count > 0)
            {
                filter.Add(new NavigationFilterItem()
                {
                    Name = Constants.Filters.Posts.Sort,
                    FilterType = NavigationFilterType.Select,
                    Values = sort
                });
            }

            return filter;
        }
        #endregion

        public AjaxForm CreateProfileImageForm(AccountUser user)
        {
            var model = new AjaxForm()
            {
                Action = $"/api/accountprofile/{user?.AccountProfileId}/image",
                Type = PostbackType.PUT,
                Event = "profile:updated",
                IsFile = true,
                Label = "Update",
                Disabled = user.Status == AccountUserStatus.Deactivated ? true : null,
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "File",
                        FieldComponent = FormFieldComponents.aclFieldFile,
                        FieldType = FormFieldTypes.Image,
                        Placeholder = "Upload image",
                        Multiple = false,
                        ClearOnSubmit = true,
                        Value = user?.AccountProfile?.Image?.ToString(),
                        Icon = "photo_camera",
                        AriaInvalid = false,
                        Hidden = false,
                        Accept = ".png,.jpg",
                    },
                    new FormField()
                    {
                        Name = "Id",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        Value = user?.AccountProfileId,
                    }
                }
            };
            return model;
        }
        public AjaxForm CreateDeactivateForm(AccountUser user)
        {
            var model = new AjaxForm()
            {
                Action = $"/api/accountuser/deactivate",
                Type = PostbackType.POST,
                Event = "user:deactivate",
                Label = "Deactivate",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "UserId",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        Value = user.Id,
                    },
                    new FormField()
                    {
                        Name = "Username",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        Value = user?.UserName,
                    }
                }
            };
            return model;
        }

        public AjaxForm CreateDeleteForm(AccountUser user)
        {
            var model = new AjaxForm()
            {
                Action = $"/api/accountuser/delete",
                Type = PostbackType.POST,
                Event = "user:delete",
                Label = "Delete",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "UserId",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        Value = user.Id,
                    },
                    new FormField()
                    {
                        Name = "Username",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        Value = user?.UserName,
                    }
                }
            };
            return model;
        }

        public AjaxForm CreateReactivateForm(AccountUser user)
        {
            var model = new AjaxForm()
            {
                Action = $"/api/accountuser/reactivate",
                Type = PostbackType.POST,
                Event = "user:Reactivate",
                Label = "Reactivate",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "UserId",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        Value = user.Id,
                    },
                    new FormField()
                    {
                        Name = "Username",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        Value = user?.UserName,
                    }
                }
            };
            return model;
        }
        public AjaxForm CreateUserForm(AccountUser user)
        {
            var model = new AjaxForm()
            {
                Action = $"/api/accountuser/{user?.Id}",
                Type = PostbackType.PUT,
                Event = "user:create",
                Disabled = user.Status == AccountUserStatus.Deactivated ? true : null,
                Label = "Update",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Label = "Email",
                        Name = "Email",
                        FieldType = FormFieldTypes.email,
                        Disabled = true,
                        Placeholder = "Email",
                        Value = user?.Email,
                    },
                    new FormField()
                    {
                        Label = "Username",
                        Name = "Username",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Username",
                        Disabled = user.Email != user.UserName || user.Status == AccountUserStatus.Deactivated,
                        Value = user?.UserName,
                    }, 
                    new FormField()
                    {
                        Name = "Id",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        Value = user.Id,
                    }
                }
            };
            return model;
        }
        public AjaxForm CreateProfileForm(AccountUser user)
        {
            var model = new AjaxForm()
            {
                Action = $"/api/accountprofile/{user?.AccountProfileId}",
                Type = PostbackType.PUT,
                Event = "on:user:delete",
                Label = "Update",
                Disabled = user.Status == AccountUserStatus.Deactivated ? true : null,
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Label = "Firstname",
                        Name = "Firstname",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Firstname",
                        Disabled = user.Status == AccountUserStatus.Deactivated ? true : null,
                        Value = user?.AccountProfile?.Firstname,
                    },
                    new FormField()
                    {
                        Label = "Lastname",
                        Name = "Lastname",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Lastname",
                        Disabled = user.Status == AccountUserStatus.Deactivated ? true : null,
                        Value = user?.AccountProfile?.Lastname,
                    },
                    new FormField()
                    {
                        Name = "UserId",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        Value = user.Id,
                    },
                    new FormField()
                    {
                        Name = "Id",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        Value = user?.AccountProfileId,
                    }
                }
            };
            return model;
        }
        public ModalForm CreateModalMediaForm(AccountUser user)
        {
            var model = new ModalForm();
            model.Title = "Create media";
            model.Text = "Test form text";
            model.Event = "modal-create-channel";
            model.Form = CreateMediaForm(user);
            return model;
        }
        public AjaxForm CreateMediaForm(AccountUser user)
        {
            var model = new AjaxForm()
            {
                Action = "/api/mediablob/image",
                Type = PostbackType.POST,
                Disabled = user.Status == AccountUserStatus.Deactivated ? true : null,
                Event = "channel:create:modal",
                Label = "Create",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "File",
                        FieldComponent = FormFieldComponents.aclFieldFile,
                        FieldType = FormFieldTypes.file,
                        Placeholder = "Upload image",
                        Multiple = false,
                        ClearOnSubmit = true,
                        Icon = "photo_camera",
                        Disabled = user.Status == AccountUserStatus.Deactivated ? true : null,
                        AriaInvalid = false,
                        Hidden = false,
                        Accept = ".png,.jpg",
                    },
                    new FormField()
                    {
                        Name = "UserId",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        AriaInvalid = false,
                        Value = user.Id,
                    }
                }
            };
            return model;
        }
        #endregion
        #region Login
        public async Task<AccountFormPage> GetLoginPage(string? username)
        {
            var viewModel = new AccountFormPage(_contentService.CreatePageBaseContent());
            viewModel.Title = "Login";
            viewModel.Form = this.CreateLoginForm(username);
            viewModel.Links = CreateLoginLinks();
            viewModel.ExternalLoginAction = "ExternalLogin";
            viewModel.ExternalLoginPostbackUrl = "/Account/ExternalLogin";
            var providers = await _signInManager.GetExternalAuthenticationSchemesAsync();
            viewModel.Providers = this.GetProviderLinks(providers);
            return viewModel;
        }
        private List<ProviderLink> GetProviderLinks(IEnumerable<AuthenticationScheme> providers)
        {
            var result = new List<ProviderLink>();
            foreach (var provider in providers)
            {
                result.Add(new ProviderLink()
                {
                    Name = provider.Name,
                    Color = GetProviderColor(provider.Name)
                });
            }
            return result;
        }
        private string GetProviderColor(string provider)
        {
            switch (provider)
            {
                case "Facebook":
                    return "#4267B2";
                case "Google":
                    return "#4285F4";
                case "Microsoft":
                    return "#000";
                default:
                    return "#4267B2";
            }
        }
        public Form CreateLoginForm(string? username)
        {
            var model = new Form()
            {
                Label = "Login",
                Type = PostbackType.POST,
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "Username",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Username",
                        AriaInvalid = false,
                        Value = username
                    },
                    new FormField()
                    {
                        Name = "Password",
                        Placeholder = "Password",
                        FieldType = FormFieldTypes.password,
                        AriaInvalid = false,
                    }
                }
            };
            return model;
        } 
        private List<NavigationItem> CreateSocialLinks()
        {
            //<a class="btn btn-primary btn-block btn-responsive" style="background-color: #3578E5; border-color: #3578E5; color: #ffffff; font-size:1.5rem; padding: 20px;" href="https://www.facebook.com/dialog/oauth?client_id=@Html.DisplayFor(model => model.FacebookAppId)&redirect_uri=@Html.DisplayFor(model => model.FacebookRedirectUri)&&response_type=code&scope=email"><i class="fab fa-facebook"></i>&nbsp;&nbsp;&nbsp;Login with Facebook</a>

            return new List<NavigationItem>()
            {
                new NavigationItem()
                {
                    Href = $"https://www.facebook.com/dialog/oauth?client_id={_OAuthConfig.FacebookAppId}&redirect_uri={_OAuthConfig.FacebookRedirectUri}&response_type=code&scope=email",
                    Text = "Facebook",
                    Class = "secondary",
                    Icon = "facebook"
                },
                /*
                 * https://accounts.google.com/o/oauth2/v2/auth?
                 scope=https%3A//www.googleapis.com/auth/drive.metadata.readonly&
                 access_type=offline&
                 include_granted_scopes=true&
                 response_type=code&
                 state=state_parameter_passthrough_value&
                 redirect_uri=https%3A//oauth2.example.com/code&
                 client_id=client_id
                */
                new NavigationItem()
                {
                    Href = $"https://accounts.google.com/o/oauth2/v2/auth?scope=https%3A//www.googleapis.com/auth/drive.metadata.readonly&access_type=offline&include_granted_scopes=true&response_type=code&state=state_parameter_passthrough_value&redirect_uri=${_OAuthConfig.GoogleRedirectUri}code&client_id=${_OAuthConfig.GoogleAppId}",
                    Text = "Google",
                    Class = "secondary",
                    Icon = "Google"
                },
            };
        }
        private List<NavigationItem> CreateLoginLinks()
        {
            return new List<NavigationItem>()
            {
                new NavigationItem()
                {
                    Href = _contentService.GetActionUrl(nameof(AccountController.Register), ControllerHelper.NameOf<AccountController>()),
                    Text = nameof(AccountController.Register),
                    Class = "secondary"
                },
                new NavigationItem()
                {
                    Href = _contentService.GetActionUrl(nameof(AccountController.ForgotPassword), ControllerHelper.NameOf<AccountController>()),
                    Text = "Forgot Password",
                    Class = "secondary",
                },
            };
        }
        #endregion
        #region ExternalLoginExistingUser
        public async Task<AccountFormPage> GetExternalLoginExistingUser(string? username, string providerName)
        {
            var viewModel = new AccountFormPage(_contentService.CreatePageBaseContent());
            viewModel.Title = "Link Account";
            viewModel.Form = this.CreatelLoginExistingUserForm(username, providerName);
            viewModel.Links = CreateLoginLinks();
            viewModel.ExternalLoginPostbackUrl = "/Account/ExternalLoginExistingUser";
            viewModel.ExternalLoginAction = "ExternalLoginExistingUser";

            return viewModel;
        }
        public Form CreatelLoginExistingUserForm(string? username, string providerName)
        {
            var model = new Form()
            {
                Label = "Link",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "Link",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Username",
                        AriaInvalid = false,
                        Disabled = true,
                        Value = $"Continue to link your {providerName} account"
                    },
                    new FormField()
                    {
                        Name = "Username",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Username",
                        AriaInvalid = false,
                        Hidden = false,
                        Disabled = true,
                        Value = username
                    }
                }
            };
            return model;
        }
        #endregion
        #region ExternalLoginNewUser
        public async Task<AccountFormPage> GetExternalLoginNewUser(string? username)
        {
            var viewModel = new AccountFormPage(_contentService.CreatePageBaseContent());
            viewModel.Title = "Complete Account";
            viewModel.Form = this.CreatelLoginNewUserForm(username);
            viewModel.Links = CreateLoginLinks();
            viewModel.ExternalLoginAction = "ExternalLogin";
            viewModel.ExternalLoginPostbackUrl = "/Account/ExternalLoginNewUser"; 

            return viewModel;
        }
        public Form CreatelLoginNewUserForm(string? username)
        {
            var model = new Form()
            {
                Title = "Create a username",
                Label = "Submit",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "Username",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Username",
                        Min = 3,
                        Max = 20,
                        AriaInvalid = false,
                        Value = username
                    },
                }
            };
            return model;
        }
        #endregion
        #region ExternalLoginDeactivatedUser
        public async Task<AccountFormPage> GetExternalLoginDeactivatedUser(string? username)
        {
            var viewModel = new AccountFormPage(_contentService.CreatePageBaseContent());
            viewModel.Title = "Reactivate Account";
            viewModel.Form = this.CreatelLoginDeactivatedUserForm(username);
            viewModel.Links = CreateLoginLinks();
            viewModel.ExternalLoginAction = "ExternalLogin";
            viewModel.ExternalLoginPostbackUrl = "/Account/ExternalLoginDeactivatedUser";

            return viewModel;
        }
        public Form CreatelLoginDeactivatedUserForm(string? username)
        {
            var model = new Form()
            {
                Title = "Activate your account",
                Label = "Continue",
                Type = PostbackType.POST,
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "Message",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Message",
                        AriaInvalid = false,
                        Disabled = true,
                        Value = "This account has been de-activated, continue to re-active this account."
                    },
                    new FormField()
                    {
                        Name = "Username",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Username",
                        AriaInvalid = false,
                        Value = username,
                        Hidden = false,
                        Disabled = true
                    },
                }
            };
            return model;
        }
        #endregion
        #region Register
        public AccountFormPage GetRegisterPage(string? username, string? email)
        {
            var viewModel = new AccountFormPage(_contentService.CreatePageBaseContent());
            viewModel.Title = "Register"; 
            viewModel.Form = this.CreateRegisterForm(username, email);
            viewModel.Links = CreateRegisterLinks();
            return viewModel;
        }
        private List<NavigationItem> CreateRegisterLinks()
        {
            return new List<NavigationItem>()
            {
                new NavigationItem()
                {
                    Href = _contentService.GetActionUrl(nameof(AccountController.Login), ControllerHelper.NameOf<AccountController>()),
                    Text = nameof(AccountController.Login),
                    Class = "secondary"
                },
                new NavigationItem()
                {
                    Href = _contentService.GetActionUrl(nameof(AccountController.ForgotPassword), ControllerHelper.NameOf<AccountController>()),
                    Text = "Forgot Password",
                    Class = "contrast",
                },
            };
        }
        public Form CreateRegisterForm(string? username, string? email)
        {
            var model = new Form()
            {
                Title = "Register by email",
                Label = "Register",
                Type = PostbackType.POST,
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "Username",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Username",
                        AriaInvalid = false,
                        Value = username
                    },
                    new FormField()
                    {
                        Name = "Email",
                        FieldType = FormFieldTypes.email,
                        Placeholder = "Email",
                        AriaInvalid = false,
                        Value = email
                    },
                    new FormField()
                    {
                        Name = "Password",
                        Placeholder = "Password",
                        FieldType = FormFieldTypes.password,
                        AriaInvalid = false,
                    },
                    new FormField()
                    {
                        Name = "ConfirmPassword",
                        Placeholder = "Confirm Password",
                        FieldType = FormFieldTypes.password,
                        AriaInvalid = false,
                    }
                }
            };
            return model;
        }
        #endregion
        #region ForgotPassword
        public AccountFormPage GetForgotPasswordConfirmationPage()
        {
            var viewModel = new AccountFormPage(_contentService.CreatePageBaseContent());
            viewModel.Title = "Forgot password";
            viewModel.Form = this.CreateForgotPasswordConfirmationForm();
            viewModel.Links = this.CreateForgotPasswordLinks();
            return viewModel;
        }
        public Form CreateForgotPasswordConfirmationForm()
        {
            var model = new Form()
            {
                Label = "Confirm Account",
            };
            return model;
        }
        public AccountFormPage GetForgotPasswordPage(string? usernameOrEmail)
        {
            var viewModel = new AccountFormPage(_contentService.CreatePageBaseContent());
            viewModel.Title = "Forgot password";
            viewModel.Form = this.CreateForgotPasswordForm(usernameOrEmail);
            viewModel.Links = this.CreateForgotPasswordLinks();
            return viewModel;
        }
        public Form CreateForgotPasswordForm(string? usernameOrEmail)
        {
            var model = new Form()
            {
                Title = "Reset your password",
                Label = "Forgot Password",
                Type = PostbackType.POST,
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "Username",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Username or email",
                        AriaInvalid = false,
                        Hidden = false,
                        Value = usernameOrEmail
                    },
                }
            };
            return model;
        }
        private List<NavigationItem> CreateForgotPasswordLinks()
        {
            return new List<NavigationItem>()
            {
                new NavigationItem()
                {
                    Href = _contentService.GetActionUrl(nameof(AccountController.Login), ControllerHelper.NameOf<AccountController>()),
                    Text = nameof(AccountController.Login),
                    Class = "secondary"
                },
                new NavigationItem()
                {
                    Href = _contentService.GetActionUrl(nameof(AccountController.Register), ControllerHelper.NameOf<AccountController>()),
                    Text = nameof(AccountController.Register),
                    Class = "contrast",
                },
            };
        }
        #endregion
        #region ConfirmAccount
        public AccountFormPage GetConfirmAccountPage(string? userId)
        {
            var viewModel = new AccountFormPage(_contentService.CreatePageBaseContent());
            viewModel.Title = "Confirm your account";
            viewModel.Form = this.CreateConfirmAccountForm(userId);
            viewModel.Links = this.CreateResetPasswordLinks();
            return viewModel;
        }
        public Form CreateConfirmAccountForm(string? userId)
        {
            var model = new Form()
            {
                Title = "Confirm your account",
                Label = "Confirm Account",
                Type = PostbackType.POST,
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "Username",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Username or email",
                        AriaInvalid = false,
                        Value = userId
                    },
                }
            };
            return model;
        }
        #endregion
        #region ResetPassword 
        public AccountFormPage GetResetPasswordPage(string? userId, string? code)
        {
            var viewModel = new AccountFormPage(_contentService.CreatePageBaseContent());
            viewModel.Title = "Reset password";
            viewModel.Form = this.CreateResetPasswordResetForm(userId, code);
            viewModel.Links = this.CreateResetPasswordLinks();
            return viewModel;
        }
        public Form CreateResetPasswordResetForm(string? userId, string? code)
        {
            var model = new Form()
            {
                Label = "Forgot Password",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "UserId",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Username or email",
                        AriaInvalid = false,
                        Hidden = true,
                        Value = userId
                    },
                    new FormField()
                    {
                        Name = "Code",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Code",
                        AriaInvalid = false,
                        Hidden = true,
                        Value = code
                    },
                    new FormField()
                    {
                        Name = "Password",
                        Label = "New Password",
                        FieldType = FormFieldTypes.password,
                        Placeholder = "New Password",
                        AriaInvalid = false,
                        Value = null
                    },
                    new FormField()
                    {
                        Name = "ConfirmPassword",
                        FieldType = FormFieldTypes.password,
                        Placeholder = "Confirm Password",
                        AriaInvalid = false,
                        Value = null
                    },
                }
            };
            return model;
        }
        private List<NavigationItem> CreateResetPasswordLinks()
        {
            return new List<NavigationItem>()
            {
                new NavigationItem()
                {
                    Href = _contentService.GetActionUrl(nameof(AccountController.Login), ControllerHelper.NameOf<AccountController>()),
                    Text = nameof(AccountController.Login),
                    Class = "secondary"
                },
                new NavigationItem()
                {
                    Href = _contentService.GetActionUrl(nameof(AccountController.Register), ControllerHelper.NameOf<AccountController>()),
                    Text = nameof(AccountController.Register),
                    Class = "contrast",
                },
            };
        }
        #endregion
        public NavigationItem? GetPageLink(string route, string? name = null)
        {
            return new NavigationItem()
            {
                Text = name ?? route,
                Href = this._contentService.GetActionUrl(route, ControllerHelper.NameOf<AccountController>(), new { })
            };
        }
    }
}

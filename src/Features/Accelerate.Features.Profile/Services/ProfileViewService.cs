using Accelerate.Features.Profile.Controllers;
using Accelerate.Features.Profile.Models.Views;
using Accelerate.Foundations.Users.Models;
using Accelerate.Foundations.Users.Models.Entities;
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
using Accelerate.Foundations.Portal.Services;

namespace Accelerate.Features.Profile.Services
{
    public class ProfileViewService : IProfileViewService
    {
        private readonly SignInManager<UsersUser> _signInManager;
        private OAuthConfiguration _OAuthConfig;
        private IMetaContentService _contentService;
        private IPortalContentService _portalContentService;
        public ProfileViewService(
            SignInManager<UsersUser> signInManager,
            IMetaContentService contentService,
            IPortalContentService portalContentService,
            IOptions<OAuthConfiguration> options)
        {
            _signInManager = signInManager;
            _contentService = contentService;
            _portalContentService = portalContentService;
            _OAuthConfig = options.Value;
        }
        #region Notifications

        public async Task<ManagePage> GetNotificationsPage(UsersUser user, IEnumerable<ContentPostActivityEntity> activities, int totalActivities)
        {
            var viewModel = new MentionPage(await GetManagePage(user));
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

        public async Task<ManagePage> GetMentionsPage(UsersUser user)
        {
            var viewModel = new MentionPage(await GetManagePage(user));
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

        public async Task<ManagePage> GetManagePage(UsersUser user)
        {
            var profile = Accelerate.Foundations.Users.Helpers.UsersHelpers.CreateUserProfile(user);

            var model = await _portalContentService.CreateAuthenticatedContent(user);

            var viewModel = new ManagePage(model);
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
                GetPageLink(nameof(ProfileController.Index)),
                //GetPageLink(nameof(ProfileController.Posts)),
                //GetPageLink(nameof(AccountController.Mentions)),
                //GetPageLink(nameof(ProfileController.Media)),
                //GetPageLink(nameof(ProfileController.Notifications)),
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

        public AjaxForm CreateProfileImageForm(UsersUser user)
        {
            var model = new AjaxForm()
            {
                Action = $"/api/UsersProfile/{user?.UsersProfileId}/image",
                Type = PostbackType.PUT,
                Event = "profile:updated",
                IsFile = true,
                Label = "Update",
                Disabled = user.Status == UsersUserStatus.Deactivated ? true : null,
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
                        Value = user?.UsersProfile?.Image?.ToString(),
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
                        Value = user?.UsersProfileId,
                    }
                }
            };
            return model;
        }
        public AjaxForm CreateDeactivateForm(UsersUser user)
        {
            var model = new AjaxForm()
            {
                Action = $"/api/UsersUser/deactivate",
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

        public AjaxForm CreateDeleteForm(UsersUser user)
        {
            var model = new AjaxForm()
            {
                Action = $"/api/UsersUser/delete",
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

        public AjaxForm CreateReactivateForm(UsersUser user)
        {
            var model = new AjaxForm()
            {
                Action = $"/api/UsersUser/reactivate",
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
        public AjaxForm CreateUserForm(UsersUser user)
        {
            var model = new AjaxForm()
            {
                Action = $"/api/UsersUser/{user?.Id}",
                Type = PostbackType.PUT,
                Event = "user:create",
                Disabled = user.Status == UsersUserStatus.Deactivated ? true : null,
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
                        Disabled = user.Email != user.UserName || user.Status == UsersUserStatus.Deactivated,
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
        public AjaxForm CreateProfileForm(UsersUser user)
        {
            var model = new AjaxForm()
            {
                Action = $"/api/UsersProfile/{user?.UsersProfileId}",
                Type = PostbackType.PUT,
                Event = "on:user:delete",
                Label = "Update",
                Disabled = user.Status == UsersUserStatus.Deactivated ? true : null,
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Label = "Firstname",
                        Name = "Firstname",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Firstname",
                        Disabled = user.Status == UsersUserStatus.Deactivated ? true : null,
                        Value = user?.UsersProfile?.Firstname,
                    },
                    new FormField()
                    {
                        Label = "Lastname",
                        Name = "Lastname",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Lastname",
                        Disabled = user.Status == UsersUserStatus.Deactivated ? true : null,
                        Value = user?.UsersProfile?.Lastname,
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
                        Value = user?.UsersProfileId,
                    }
                }
            };
            return model;
        }
        public ModalForm CreateModalMediaForm(UsersUser user)
        {
            var model = new ModalForm();
            model.Title = "Create media";
            model.Text = "Test form text";
            model.Event = "modal-create-channel";
            model.Form = CreateMediaForm(user);
            return model;
        }
        public AjaxForm CreateMediaForm(UsersUser user)
        {
            var model = new AjaxForm()
            {
                Action = "/api/mediablob/image",
                Type = PostbackType.POST,
                Disabled = user.Status == UsersUserStatus.Deactivated ? true : null,
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
                        Disabled = user.Status == UsersUserStatus.Deactivated ? true : null,
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
        public NavigationItem? GetPageLink(string route, string? name = null)
        {
            return new NavigationItem()
            {
                Text = name ?? route,
                Href = this._contentService.GetActionUrl(route, ControllerHelper.NameOf<ProfileController>(), new { })
            };
        }
    }
}

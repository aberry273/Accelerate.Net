using Accelerate.Features.Content.Controllers;
using Accelerate.Features.Content.Models.UI;
using Accelerate.Features.Content.Models.Views;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Helpers;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Aggregations;
using System.Drawing;

namespace Accelerate.Features.Content.Services
{
    public class ContentViewService : IContentViewService
    {
        IMetaContentService _metaContentService;
        public ContentViewService(IMetaContentService metaContent)
        {
            _metaContentService = metaContent;
        }

        private BasePage CreateBaseContent(AccountUser user)
        {
            var profile = user != null ? new UserProfile()
            {
                Username = user.UserName,
                Image = user?.AccountProfile?.Image
            } : null;
            return _metaContentService.CreatePageBaseContent(profile);
        }
        public ChannelsPage CreateAnonymousChannelsPage()
        {
            var model = CreateBaseContent(null);
            var viewModel = new ChannelsPage(model);
            viewModel.ChannelsDropdown = GetChannelsDropdown();

            viewModel.UserId = Guid.Empty;
            return viewModel;
        }
        public ChannelsPage CreateChannelsPage(AccountUser user, SearchResponse<ContentChannelDocument> channels)
        {
            var model = CreateBaseContent(user);
            var viewModel = new ChannelsPage(model);
            viewModel.ChannelsDropdown = GetChannelsDropdown();

            if (channels != null && channels.IsValidResponse)
            {
                var channelItems = channels.Documents.Select(x => new NavigationItem()
                {
                    Text = x.Name,
                    Href = this._metaContentService.GetActionUrl(nameof(ChannelsController.Channel), ControllerHelper.NameOf<ChannelsController>(), new { id = x.Id })
                });
                viewModel.ChannelsDropdown.Items.AddRange(channelItems);
            }

            viewModel.UserId = user != null ? user.Id : Guid.Empty;
            viewModel.FormCreatePost = user != null ? CreatePostForm(user) : null;
            viewModel.ModalCreateChannel = CreateModalChannelForm(user);
            viewModel.ModalEditReply = CreateModalEditReplyForm(user);
            viewModel.ModalDeleteReply = CreateModalDeleteReplyForm(user);
            return viewModel;
        }
        public ChannelPage CreateChannelPage(AccountUser user, ContentChannelDocument item, SearchResponse<ContentChannelDocument> channels, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var model = CreateBaseContent(user);
            var viewModel = new ChannelPage(model);

            viewModel.Item = item;
            viewModel.Tabs = new List<NavigationItem>()
            {
                new NavigationItem()
                {
                    Text = "All",
                    Href = $"{this.GetChannelUrl(item)}/All",
                },
                new NavigationItem()
                {
                    Text = "Posts",
                    Href = $"{this.GetChannelUrl(item)}/Posts",
                },
                new NavigationItem()
                {
                    Text = "Related",
                    Href = $"{this.GetChannelUrl(item)}/Related",
                },
                /*
                new NavigationItem()
                {
                    Text = "Media",
                    Href = $"{this.GetChannelUrl(item)}/Media",
                },
                new NavigationItem()
                {
                    Text = "Users",
                    Href = $"{this.GetChannelUrl(item)}/Users",
                }*/
            };

            viewModel.ChannelsDropdown = GetChannelsDropdown(channels, item.Name);

            // Add filters
            viewModel.Filters = CreateSearchFilters(aggregateResponse);

            viewModel.UserId = user.Id;
            viewModel.FormCreateReply = CreatePostForm(user, item);
            viewModel.ModalEditChannel = EditModalChannelForm(user, item);
            viewModel.ModalEditReply = CreateModalEditReplyForm(user);
            viewModel.ModalDeleteReply = CreateModalDeleteReplyForm(user);
            return viewModel;
        }
        public ThreadPage CreateThreadPage(AccountUser user, ContentPostDocument item, SearchResponse<ContentPostDocument> aggregateResponse, SearchResponse<ContentPostDocument> replies, ContentChannelDocument? channel = null)
        {
            var model = CreateBaseContent(user);
            var viewModel = new ThreadPage(model);
            viewModel.Item = item;
            #pragma warning disable CS8601 // Possible null reference assignment.
            viewModel.ParentLink = GetThreadLink(item.TargetThread, item.ParentId);
            #pragma warning restore CS8601 // Possible null reference assignment.
            viewModel.ChannelLink = GetChannelLink(channel);

            viewModel.UserId = user.Id;
            // Get replies
            //var replies = await _postSearchService.Search(GetRepliesQuery(item), 0, 1000);
            viewModel.Replies = replies.Documents.ToList();
            viewModel.FormCreateReply = CreateReplyForm(user, item);
            viewModel.ModalEditReply = CreateModalEditReplyForm(user);
            viewModel.ModalDeleteReply = CreateModalDeleteReplyForm(user);

            // Add filters
            viewModel.Filters = CreateSearchFilters(aggregateResponse);

            return viewModel;
        }
        public NotFoundPage CreateNotFoundPage(AccountUser user, string title, string description)
        {
            var model = CreateBaseContent(user);
            var viewModel = new NotFoundPage(model);
            viewModel.Title = "Post not found";
            viewModel.Description = "We are unable to retrieve this post, this may have been deleted or made private.";
            return viewModel;
        }
        public AjaxForm CreatePostForm(AccountUser user, ContentChannelDocument channel = null)
        {
            var model = new AjaxForm()
            {
                PostbackUrl = "/api/contentpost/mixed",
                Type = PostbackType.POST,
                Event = "post:created",
                Label = "Submit",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "Content",
                        FieldType = FormFieldTypes.textarea,
                        Placeholder = "Post an update",
                        ClearOnSubmit = true,
                        AriaInvalid = false
                    },
                    new FormField()
                    {
                        Name = "Status",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        AriaInvalid = false,
                        Value = ContentPostEntityStatus.Public,
                    },
                    new FormField()
                    {
                        Name = "Category",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Add tag",
                        ClearOnSubmit = true,
                        AriaInvalid = false,
                        Hidden = true,
                        Value = channel?.Category
                    },
                    new FormField()
                    {
                        Name = "Tags",
                        FieldType = FormFieldTypes.chips,
                        Placeholder = "Add tag",
                        ClearOnSubmit = true,
                        AriaInvalid = false,
                        Hidden = false,
                        Value = channel?.Tags
                    },
                    new FormField()
                    {
                        Name = "Images",
                        FieldType = FormFieldTypes.file,
                        Placeholder = "Upload image",
                        Multiple = true,
                        ClearOnSubmit = true,
                        Icon = "photo_camera",
                        AriaInvalid = false,
                        Hidden = true,
                    },
                    new FormField()
                    {
                        Name = "Video",
                        FieldType = FormFieldTypes.file,
                        Placeholder = "Upload video",
                        ClearOnSubmit = true,
                        Icon = "photo_camera",
                        AriaInvalid = false,
                        Hidden = true,
                    },
                    new FormField()
                    {
                        Name = "UserId",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        AriaInvalid = false,
                        Value = user.Id,
                    },
                    new FormField()
                    {
                        Name = "Type",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        AriaInvalid = false,
                        Value = ContentPostType.Post,
                    }
                }
            };
            if(channel != null )
            {
                model.Fields.Add(
                    new FormField()
                    {
                        Name = "TargetChannel",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        AriaInvalid = false,
                        Value = channel.Id,
                    });
            }
            return model;
        }
        public AjaxForm CreateReplyForm(AccountUser user, ContentPostDocument post)
        {
            var model = new AjaxForm()
            {
                PostbackUrl = "/api/contentpost/mixed",
                Type = PostbackType.POST,
                Event = "post:created",
                ActionEvent = "action:post",
                Label = "Reply",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name= "QuoteIds",
                        FieldType = FormFieldTypes.list,
                        Class = "flat",
                        Placeholder = "Quotes",
                        IsArray = true,
                        Autocomplete = null,
                        ClearOnSubmit = true,
                        AriaInvalid = true,
                        Hidden = false,
                        Helper = "",
                    },
                    new FormField()
                    {
                        Name = "Content",
                        FieldType = FormFieldTypes.textarea,
                        Placeholder = "Post a reply",
                        ClearOnSubmit = true,
                        AriaInvalid = false
                    },
                    new FormField()
                    {
                        Name = "Images",
                        FieldType = FormFieldTypes.file,
                        Placeholder = "Upload image",
                        Multiple = true,
                        ClearOnSubmit = true,
                        Icon = "photo_camera",
                        AriaInvalid = false,
                        Hidden = true,
                    },
                    new FormField()
                    {
                        Name = "Video",
                        FieldType = FormFieldTypes.file,
                        Placeholder = "Upload video",
                        ClearOnSubmit = true,
                        Icon = "photo_camera",
                        AriaInvalid = false,
                        Hidden = true,
                    },
                    new FormField()
                    {
                        Name = "Tags",
                        FieldType = FormFieldTypes.chips,
                        Placeholder = "Add a tag",
                        ClearOnSubmit = true,
                        AriaInvalid = false,
                        Hidden = post.Tags != null,
                        Value = post.Tags
                    },
                    new FormField()
                    {
                        Name = "UserId",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        AriaInvalid = false,
                        ClearOnSubmit = false,
                        Value = user.Id,
                    },
                    new FormField()
                    {
                        Name = "ParentId",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        AriaInvalid = false,
                        ClearOnSubmit = false,
                        Value = post.Id,
                    },
                    new FormField()
                    {
                        Name = "Type",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        AriaInvalid = false,
                        Value = ContentPostType.Reply,
                    },
                    new FormField()
                    {
                        Name = "TargetThread",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        AriaInvalid = false,
                        ClearOnSubmit = false,
                        Value = post.ThreadId,
                    },
                    new FormField()
                    {
                        Name = "TargetChannel",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        AriaInvalid = false,
                        Value = post.TargetChannel,
                    }, 
                    new FormField()
                    {
                        Name = "Category",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        AriaInvalid = false,
                        Value = post.Category,
                    },
                    new FormField()
                    {
                        Name = "Status",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        AriaInvalid = false,
                        Value = ContentPostEntityStatus.Public,
                    },
                }
            };
            return model;
        }

        public ModalForm EditModalChannelForm(AccountUser user, ContentChannelDocument channel)
        {
            var model = new ModalForm();
            model.Title = "Edit channel";
            model.Text = "Test form text";
            model.Target = "modal-create-channel";
            model.Form = EditChannelForm(user, channel);
            return model;
        }
        public AjaxForm EditChannelForm(AccountUser user, ContentChannelDocument channel)
        {
            var model = new AjaxForm()
            {
                PostbackUrl = $"/api/contentchannel/{channel.Id}",
                Type = PostbackType.PUT,
                Event = "channel:edit:modal",
                Label = "Update",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "Name",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Channel name",
                        AriaInvalid = false,
                        Value = channel.Name
                    },
                    new FormField()
                    {
                        Name = "Status",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        AriaInvalid = false,
                        Value = ContentChannelEntityStatus.Public,
                    },
                    new FormField()
                    {
                        Name = "Category",
                        FieldType = FormFieldTypes.input,
                        Hidden = false,
                        Disabled = false,
                        AriaInvalid = false,
                        Value = channel.Category
                    },
                    new FormField()
                    {
                        Name = "TagItems",
                        Label = "Tags",
                        FieldType = FormFieldTypes.chips,
                        Placeholder = "Listen to posts tagged with..",
                        ClearOnSubmit = true,
                        AriaInvalid = false,
                        Hidden = false,
                        Value = channel.Tags
                    },
                    new FormField()
                    {
                        Name = "Description",
                        FieldType = FormFieldTypes.textarea,
                        Placeholder = "Describe what content this channel is for",
                        AriaInvalid = false,
                        Value = channel.Description
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
        public AjaxForm CreateChannelForm(AccountUser user)
        {
            var model = new AjaxForm()
            {
                PostbackUrl = "/api/contentchannel",
                Type = PostbackType.POST,
                Event = "channel:create:modal",
                Label = "Create",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "Name",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Channel name",
                        AriaInvalid = false
                    },
                    new FormField()
                    {
                        Name = "Status",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        AriaInvalid = false,
                        Value = ContentChannelEntityStatus.Public,
                    },
                    new FormField()
                    {
                        Name = "Category",
                        FieldType = FormFieldTypes.input,
                        Hidden = false,
                        Disabled = false,
                        AriaInvalid = false,
                    },
                    new FormField()
                    {
                        Name = "TagItems",
                        Label = "Tags",
                        FieldType = FormFieldTypes.chips,
                        Placeholder = "Listen to posts tagged with..",
                        ClearOnSubmit = true,
                        AriaInvalid = false,
                        Hidden = false,
                    },
                    new FormField()
                    {
                        Name = "Description",
                        FieldType = FormFieldTypes.textarea,
                        Placeholder = "Describe what content this channel is for",
                        AriaInvalid = false
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
        public ModalForm CreateModalChannelForm(AccountUser user)
        {
            var model = new ModalForm();
            model.Title = "Create channel";
            model.Text = "Test form text";
            model.Target = "modal-create-channel";
            model.Form = CreateChannelForm(user);
            return model;
        }
        public ModalForm CreateModalEditReplyForm(AccountUser user)
        {
            var model = new ModalForm();
            model.Title = "Edit post";
            model.Text = "Test form text";
            model.Target = "modal-edit-post";
            model.Form = CreateFormEditReply(user);
            return model;
        }
        public ModalForm EditModalReplyForm(AccountUser user)
        {
            var model = new ModalForm();
            model.Title = "Edit post";
            model.Text = "Test form text";
            model.Target = "modal-edit-post";
            model.Form = CreateFormEditReply(user);
            return model;
        }
        public AjaxForm CreateFormEditReply(AccountUser user)
        {
            var model = new AjaxForm()
            {
                PostbackUrl = "/api/contentpost",
                Type = PostbackType.PUT,
                Event = "post:edited:modal",
                Label = "Comment",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "Content",
                        FieldType = FormFieldTypes.textarea,
                        Placeholder = "What\'s your update?",
                        AriaInvalid = false
                    },
                    new FormField()
                    {
                        Name = "Id",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        AriaInvalid = false,
                        Value = null,
                    },
                    new FormField()
                    {
                        Name = "Status",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        AriaInvalid = false,
                        Value = ContentPostEntityStatus.Public,
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

        public ModalForm CreateModalDeleteReplyForm(AccountUser user)
        {
            var model = new ModalForm();
            model.Title = "Delete post";
            model.Text = "Testdelete form text";
            model.Target = "modal-delete-post";
            model.Form = CreateFormDeleteReply(user);
            return model;
        }
        public AjaxForm CreateFormDeleteReply(AccountUser user)
        {
            var model = new AjaxForm()
            {
                PostbackUrl = "/api/contentpost",
                Type = PostbackType.DELETE,
                Event = "post:deleted:modal",
                Label = "Delete",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "Id",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        AriaInvalid = false,
                        Value = null,
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
        
        public Dictionary<string, string> GetFilterOptions()
        {
            return new Dictionary<string, string>()
            {
                {
                    Constants.Filters.Tags,
                    Foundations.Content.Constants.Fields.Tags
                },
                {
                    Constants.Filters.Threads,
                    Foundations.Content.Constants.Fields.ShortThreadId
                },
                {
                    Constants.Filters.Quotes,
                    Foundations.Content.Constants.Fields.QuoteIds
                }
            };
        }


        // NAVIGATION
        public List<NavigationFilter> CreateSearchFilters(SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var filterValues = new Dictionary<string, List<string>>();
            if (aggregateResponse.IsValidResponse)
            {
                var filterOptions = GetFilterOptions();
                filterValues = filterOptions.Values.ToDictionary(x => x, x => GetValuesFromAggregate(aggregateResponse.Aggregations, x));
            }
            return CreateNavigationFilters(filterValues);
        }
        private List<string> GetValuesFromAggregate(AggregateDictionary aggregates, string key)
        {
            var agg = aggregates.FirstOrDefault(x => x.Key == key);
            StringTermsAggregate vals = agg.Value as StringTermsAggregate;
            if (vals == null || vals.Buckets == null || vals.Buckets.Count == 0) return new List<string>();

            var results = vals.Buckets.
                Select(x => x.Key.Value.ToString()).
                Where(x => !string.IsNullOrEmpty(x)).
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
            var keyVal = this.GetFilterOptions().FirstOrDefault(x => x.Key == key);
            if (keyVal.Value == null) return key.ToCamelCase();
            return keyVal.Value?.ToCamelCase();
        }
        private List<string> GetAggregateValues(IDictionary<string, List<string>> aggFilters, string key)
        {
            if (key == null) return new List<string>();
            return aggFilters.ContainsKey(key) ? aggFilters[key] : new List<string>();
        }
        private List<NavigationFilter> CreateNavigationFilters(IDictionary<string, List<string>> filters)
        { 
            if(filters == null) filters = new Dictionary<string, List<string>>();
            var filter = new List<NavigationFilter>();

            var reviews = GetAggregateValues(filters, GetFilterKey(Constants.Filters.Reviews));
            if(reviews.Count > 0)
            {
                filter.Add(new NavigationFilter()
                {
                    Name = Constants.Filters.Reviews,
                    FilterType = NavigationFilterType.Select,
                    Values = reviews
                });
            }
            var threads = GetAggregateValues(filters, GetFilterKey(Constants.Filters.Threads));
            if (threads.Count > 0)
            {
                filter.Add(new NavigationFilter()
                {
                    Name = Constants.Filters.Threads,
                    FilterType = NavigationFilterType.Checkbox,
                    Values = threads
                });
            }
            var quotes = GetAggregateValues(filters, GetFilterKey(Constants.Filters.Quotes));
            if (quotes.Count > 0)
            {
                filter.Add(new NavigationFilter()
                {
                    Name = Constants.Filters.Quotes,
                    FilterType = NavigationFilterType.Checkbox,
                    Values = quotes
                });
            }

            var tags = GetAggregateValues(filters, GetFilterKey(Constants.Filters.Tags));
            if (tags.Count > 0)
            {
                filter.Add(new NavigationFilter()
                {
                    Name = Constants.Filters.Tags,
                    FilterType = NavigationFilterType.Checkbox,
                    Values = tags
                });
            }

            var content = GetAggregateValues(filters, GetFilterKey(Constants.Filters.Content));
            if (content.Count > 0)
            {
                filter.Add(new NavigationFilter()
                {
                    Name = Constants.Filters.Content,
                    FilterType = NavigationFilterType.Select,
                    Values = content
                });
            }

            var sort = GetAggregateValues(filters, GetFilterKey(Constants.Filters.Sort));
            if (sort.Count > 0)
            {
                filter.Add(new NavigationFilter()
                {
                    Name = Constants.Filters.Sort,
                    FilterType = NavigationFilterType.Select,
                    Values = sort
                });
            }

            return filter;
        }

        public NavigationGroup GetChannelsDropdown(SearchResponse<ContentChannelDocument> searchResponse = null, string selectedName = null)
        {
            var model = new NavigationGroup()
            {
                Title = selectedName ?? "All",
                Items = new List<NavigationItem>()
                {
                    new NavigationItem()
                    {
                        Text = "All",
                        Href = this._metaContentService.GetActionUrl(nameof(ChannelsController.Index), ControllerHelper.NameOf<ChannelsController>())
                    }
                }
            };
            if (searchResponse != null && searchResponse.IsValidResponse)
            {
                var channelItems = searchResponse.Documents.Select(GetChannelLink);
                model.Items.AddRange(channelItems);
            }
            return model;
        }
         
        public NavigationItem? GetThreadLink(string parentName, Guid? parentId)
        {
            if(parentId  == null) { return null; }
            return new NavigationItem()
            {
                Text = parentName,
                Href = this._metaContentService.GetActionUrl(nameof(ThreadsController.Thread), ControllerHelper.NameOf<ThreadsController>(), new { id = parentId })
            };
        }
        public NavigationItem? GetChannelLink(ContentChannelDocument x)
        {
            if (x == null) { return null; }
            return new NavigationItem()
            {
                Text = x.Name,
                Href = this._metaContentService.GetActionUrl(nameof(ChannelsController.Channel), ControllerHelper.NameOf<ChannelsController>(), new {id = x.Id })
            };
        }
        private string GetChannelUrl(ContentChannelDocument x)
        {
            return this._metaContentService.GetActionUrl(nameof(ChannelsController.Channel), ControllerHelper.NameOf<ChannelsController>(), new { id = x.Id });
        }

    }
}

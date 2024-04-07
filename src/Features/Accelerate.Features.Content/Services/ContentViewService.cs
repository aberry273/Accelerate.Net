using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Aggregations;
using Elastic.Clients.Elasticsearch.QueryDsl;
using System.Threading.Channels;
using static Accelerate.Features.Content.Constants;

namespace Accelerate.Features.Content.Services
{
    public class ContentViewService : IContentViewService
    {
        public AjaxForm CreatePostForm(AccountUser user, ContentChannelDocument channel = null)
        {
            var model = new AjaxForm()
            {
                PostbackUrl = "https://localhost:7220/api/contentpost",
                Type = PostbackType.POST,
                Event = "post:created",
                Label = "Comment",
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
                        Name = "UserId",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        AriaInvalid = false,
                        Value = user.Id,
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
                PostbackUrl = "https://localhost:7220/api/contentpost",
                Type = PostbackType.POST,
                Event = "post:created",
                Label = "Reply",
                Fields = new List<FormField>()
                {
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
                        Name = "TagItems",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        AriaInvalid = false,
                        Value = post.Tags,
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
                        Value = Enum.GetName(ContentPostEntityStatus.Public),
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
        public AjaxForm CreateChannelForm(AccountUser user)
        {
            var model = new AjaxForm()
            {
                PostbackUrl = "https://localhost:7220/api/contentchannel",
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
        public ModalForm CreateModalEditReplyForm(AccountUser user)
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
                PostbackUrl = "https://localhost:7220/api/contentpost",
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
                PostbackUrl = "https://localhost:7220/api/contentpost",
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
        
        public List<string> GetFilterOptions()
        {
            return new List<string>
            {
                Foundations.Content.Constants.Fields.Tags.ToCamelCase(),
                Foundations.Content.Constants.Fields.ThreadId.ToCamelCase()
            };
        }


        // NAVIGATION
        public List<NavigationFilter> CreateSearchFilters(SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var filterValues = new Dictionary<string, List<string>>();
            if (aggregateResponse.IsValidResponse)
            {
                var filterOptions = GetFilterOptions();
                filterValues = filterOptions.ToDictionary(x => x, x => GetValuesFromAggregate(aggregateResponse.Aggregations, x));
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
        private List<string> GetFilterKey(IDictionary<string, List<string>> filters, string key)
        {
            key = key.ToCamelCase();
            return filters.ContainsKey(key) ? filters[key] : new List<string>();
        }
        private List<NavigationFilter> CreateNavigationFilters(IDictionary<string, List<string>> filters)
        { 
            if(filters == null) filters = new Dictionary<string, List<string>>();
            var filter = new List<NavigationFilter>();

            var reviews = GetFilterKey(filters, Constants.Filters.Reviews);
            if(reviews.Count > 0)
            {
                filter.Add(new NavigationFilter()
                {
                    Name = Constants.Filters.Reviews,
                    FilterType = NavigationFilterType.Select,
                    Values = GetFilterKey(filters, Constants.Filters.Reviews)
                });
            }
            var threads = GetFilterKey(filters, Constants.Filters.Threads);
            if (threads.Count > 0)
            {
                filter.Add(new NavigationFilter()
                {
                    Name = Constants.Filters.Threads,
                    FilterType = NavigationFilterType.Checkbox,
                    Values = GetFilterKey(filters, Constants.Filters.Threads)
                });
            }

            var tags = GetFilterKey(filters, Constants.Filters.Tags);
            if (tags.Count > 0)
            {
                filter.Add(new NavigationFilter()
                {
                    Name = Constants.Filters.Tags,
                    FilterType = NavigationFilterType.Checkbox,
                    Values = GetFilterKey(filters, Constants.Filters.Tags)
                });
            }

            var content = GetFilterKey(filters, Constants.Filters.Content);
            if (content.Count > 0)
            {
                filter.Add(new NavigationFilter()
                {
                    Name = Constants.Filters.Content,
                    FilterType = NavigationFilterType.Select,
                    Values = GetFilterKey(filters, Constants.Filters.Content)
                });
            }

            var sort = GetFilterKey(filters, Constants.Filters.Sort);
            if (sort.Count > 0)
            {
                filter.Add(new NavigationFilter()
                {
                    Name = Constants.Filters.Sort,
                    FilterType = NavigationFilterType.Select,
                    Values = GetFilterKey(filters, Constants.Filters.Sort)
                });
            }

            return filter;
        }

        public NavigationGroup GetChannelsDropdown(string allChannelsUrl, SearchResponse<ContentChannelDocument> searchResponse = null, string selectedName = null)
        {
            var model = new NavigationGroup()
            {
                Title = selectedName ?? "All",
                Items = new List<NavigationItem>()
                {
                    new NavigationItem()
                    {
                        Text = "All",
                        Href = allChannelsUrl
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
        
        public NavigationItem GetChannelLink(ContentChannelDocument x)
        {
            return new NavigationItem()
            {
                Text = x.Name,
                Href = "/Content/Channels/" + x.Id
            };
        }

    }
}

using Accelerate.Features.Content.Controllers;
using Accelerate.Features.Content.Models.UI;
using Accelerate.Features.Content.Models.Views;
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Helpers;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Content.Models.View;
using Accelerate.Foundations.Content.Services;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch;
using System.Collections.Generic;

namespace Accelerate.Features.Content.Services
{
    public class ContentPostEntityViewService : BaseContentEntityViewService<ContentPostDocument>
    {
        IContentPostElasticService _contentElasticSearchService;
        public ContentPostEntityViewService(
            IMetaContentService metaContent,
            IContentViewSearchService viewSearchService,
            IContentPostElasticService contentElasticSearchService,
            IElasticService<ContentPostDocument> searchService)
            : base(metaContent, viewSearchService, searchService)
        {
            EntityName = "Post";
            _contentElasticSearchService = contentElasticSearchService;
        }
        public override ContentPostPage CreateIndexPage(UsersUser user, SearchResponse<ContentPostDocument> items, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var model = base.CreateIndexPage(user, items, aggregateResponse);
            var viewModel = new ContentPostPage(model);
            
            return viewModel;
        }
        public override async Task<ContentBasePage> CreateEntityPage(UsersUser user, ContentPostDocument item, SearchResponse<ContentPostDocument> items, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var model = await base.CreateEntityPage(user, item, items, aggregateResponse);
            var viewModel = new ContentPostPage(model);

            var filters = new List<QueryFilter>()
            {
                _searchService.Filter(Foundations.Content.Constants.Fields.ParentId, ElasticCondition.Filter, item.Id)
            };
            var aggResponse = await this._searchService.GetAggregates(_contentElasticSearchService.CreateThreadAggregateQuery(filters));
            var query = new RequestQuery()
            {
                Page = 0,
                ItemsPerPage = 100
            };
            var parentResults = await _contentElasticSearchService.SearchPostParents(query, item.Id, user?.Id);
            viewModel.Thread = await _viewSearchService.UpdatePostDocuments(user.Id, parentResults.Posts?.ToList());
            viewModel.Replies = aggResponse.Documents.Select(_contentElasticSearchService.CreateViewModel).ToList();

            var viewDoc = _contentElasticSearchService.CreateViewModel(item);

            //viewModel.PageActions = CreatePageActionsGroup(sectionName, pageName);

            // viewModel.Filters = _viewSearchService.CreateNavigationFilters(aggregateResponse);
            viewModel.UserId = user.Id;
            viewModel.FormCreatePost = CreateReplyForm(user, viewDoc);

            viewModel.ModalCreateReply = CreateModalCreateReplyForm(user, viewDoc);
            //viewModel.ModalCreateLabel = CreateModalChannelForm(user);
            //viewModel.ModalEditReply = CreateModalEditReplyForm(user);
            //viewModel.ModalDeleteReply = CreateModalDeleteReplyForm(user);
            viewModel.ActionsApiUrl = "/api/contentpostactions";
            viewModel.PostsApiUrl = "/api/contentsearch/posts";
            viewModel.FilterEvent = "filter:update";
            viewModel.ActionEvent = "action:post";

            //
            return viewModel;
        }
        public override async Task<ContentBasePage> CreateAllPage(UsersUser user, SearchResponse<ContentPostDocument> items, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var model = await base.CreateAllPage(user, items, aggregateResponse);
            var viewModel = new ContentPostPage(model);
            var viewModels = items.IsSuccess()
                ? items.Documents.Select(_viewSearchService.CreatePostViewModel).ToList()
                : new List<ContentPostViewDocument>();
            var docs = await _viewSearchService.UpdatePostDocuments(user.Id, viewModels);
            
            viewModel.Listing = new AclAjaxListing<ContentPostViewDocument>()
            {
                Items = docs != null && docs.Any()
                    ? docs
                    : new List<ContentPostViewDocument>()
            };
            
            return viewModel;
        }
        public ModalCreateContentPostReply CreateModalCreateReplyForm(UsersUser user, ContentPostViewDocument post)
        {
            var model = new ModalCreateContentPostReply();
            model.Title = "Reply to post";
            model.Text = "Test form text";
            model.IsAuthenticated = user != null;
            model.Event = "on:comment:reply";
            model.ApiUrl = "/api/contentsearch/post";
            //model.Item = post;
            model.Form = new SocialPostFormModel()
            {
                Form = CreateReplyForm(user, post),
                Actions = new List<object>(),
                FormatActions = new List<string>()
            };
            return model;
        }

        public ContentSubmitForm OldCreatePostForm(UsersUser user, ContentPostViewDocument item = null, ContentChannelDocument channel = null)
        {
            var model = new ContentSubmitForm()
            {
                //FixTop = true,
                UserId = user.Id,
                //SearchUsersUrl = "/api/accountsearch/users",
                //FetchMetadataUrl = "/api/contentpost/metadata",
                Action = "/api/contentpost/mixed",
                Type = PostbackType.POST,
                Event = "post:created",
                ActionEvent = "action:post",
                Label = "Submit",
                Fields = new List<FormField>()
                {
                    FormFieldMentions(item),
                    FormFieldQuotes(item),
                    FormFieldCharLimit(item),
                    FormFieldImageLimit(item),
                    FormFieldVideoLimit(item),
                    FormFieldContentText(item),
                    FormFieldContentFormats(item, "Formats"),
                    FormFieldStatusSelect(),
                    FormFieldLink(item),
                    FormFieldImages(item),
                    FormFieldVideos(item),
                    FormFieldTags(item),
                    FormFieldCategory(item),
                    FormFieldUser(user.Id),
                    FormFieldType(ContentPostType.Post),
                }
            };
            if (channel != null)
            {
                model.Fields.Add(FormFieldChannel(channel.Id));
            }
            if (item != null)
            {
                model.Fields.Add(FormFieldId(item.Id));
            }
            return model;
        }
        public ContentSubmitForm OldCreateReplyForm(UsersUser user, ContentPostViewDocument post)
        {
            //var parentIdThread = post.Related.Parents != null ? post.Related.Parents : new List<Guid>();
            //parentIdThread.Add(post.Id);
            var model = new ContentSubmitForm()
            {
                Id = $"form-reply-{post.Id}",
                UserId = user.Id,
                SearchUsersUrl = "/api/accountsearch/users",
                FetchMetadataUrl = "/api/contentpost/metadata",
                Action = "/api/contentpost/mixed",
                Type = PostbackType.POST,
                Event = "post:created",
                ActionEvent = "action:post",
                Label = "Reply",
                Fields = new List<FormField>()
                {
                    //FormFieldReplyTo(post),
                    FormFieldMentions(post),
                    FormFieldQuotes(post),
                    FormFieldCharLimit(post),
                    FormFieldImageLimit(post),
                    FormFieldVideoLimit(post),
                    FormFieldContentFormats(post, "Formats", "acl-content-reply"),
                    FormFieldLink(post),
                    FormFieldImages(post),
                    FormFieldVideos(post),
                    FormFieldTags(post),
                    FormFieldCategory(post),
                    FormFieldUser(user.Id),
                    //FormFieldParents(parentIdThread),
                    FormFieldParent(post),
                    FormFieldChannel(null),//post?.ChannelId
                    FormFieldStatus(post),
                }
            };
            return model;
        }
        #region FormFields Post
        private FormField FormFieldId(Guid id)
        {
            return new FormField()
            {
                Name = "Id",
                FieldType = FormFieldTypes.input,
                Hidden = true,
                Disabled = true,
                AriaInvalid = false,
                ClearOnSubmit = false,
                Value = id
            };
        }
        private FormField FormFieldReplyTo(ContentPostViewDocument post)
        {
            return new FormField()
            {
                Name = "ReplyTo",
                FieldType = FormFieldTypes.input,
                Hidden = false,
                Disabled = true,
                AriaInvalid = false,
                ClearOnSubmit = false,
                Value = GetContentPostReplyValue(post),
            };
        }
        private FormField FormFieldMentions(ContentPostDocument post)
        {
            return new FormField()
            {
                Name = "MentionItems",
                FieldType = FormFieldTypes.input,
                Class = "flat",
                Placeholder = "Mentions",
                IsArray = true,
                Autocomplete = null,
                Multiple = true,
                ClearOnSubmit = true,
                AriaInvalid = true,
                Disabled = true,
                Hidden = true,
                Helper = "",
            };
        }
        private FormField FormFieldQuotes(ContentPostDocument? post)
        {
            return new FormField()
            {
                Name = "QuotedIds",
                //FieldType = FormFieldTypes.quotes,
                Class = "flat",
                Placeholder = "Quotes",
                IsArray = true,
                Multiple = true,
                Hidden = true,
                Autocomplete = null,
                ClearOnSubmit = true,
                AriaInvalid = true,
                Helper = "",
            };
        }
        private FormField FormFieldCharLimit(ContentPostDocument? post)
        {
            return new FormField()
            {
                Name = "CharLimit",
                FieldType = FormFieldTypes.number,
                Class = "flat",
                Placeholder = "Character Limit",
                Autocomplete = null,
                ClearOnSubmit = true,
                Min = 1,
                Max = 1028,
                AriaInvalid = true,
                Hidden = true,
            };
        }
        private FormField FormFieldImageLimit(ContentPostDocument? post)
        {
            return new FormField()
            {
                Name = "Image",
                FieldType = FormFieldTypes.number,
                Class = "flat",
                Placeholder = "Image Limit",
                Autocomplete = null,
                ClearOnSubmit = true,
                Min = 1,
                Max = 4,
                AriaInvalid = true,
                Hidden = true,
            };
        }
        private FormField FormFieldVideoLimit(ContentPostDocument? post)
        {
            return new FormField()
            {
                Name = "Video",
                FieldType = FormFieldTypes.number,
                Class = "flat",
                Placeholder = "Video Limit",
                Autocomplete = null,
                ClearOnSubmit = true,
                Min = 1,
                Max = 4,
                AriaInvalid = true,
                Hidden = true,
            };
        }
        private FormField FormFieldContentText(ContentPostDocument post)
        {
            return new FormField()
            {
                Name = "Text",
                FieldComponent = FormFieldComponents.aclFieldTextarea,
                Event = "form:input:user",
                Placeholder = "Post something..",
                Hidden = true,
                ClearOnSubmit = true,
                AriaInvalid = false,
                //Max = post?.Settings?.CharLimit ?? 512,
            };
        }
        private FormField FormFieldContentFormats(ContentPostViewDocument post, string name = "Formats", string id = "aclFieldEditorJs")
        {
            return new FormField()
            {
                Id = id,
                Name = name,
                FieldComponent = FormFieldComponents.aclFieldEditorJs,
                Event = "form:input:user",
                Placeholder = "Write some content",
                ClearOnSubmit = true,
                AriaInvalid = false,
                //Max = post?.Settings?.CharLimit ?? 512,
            };
        }
        private string GetContentPostReplyValue(ContentPostViewDocument post)
        {
            if (post?.Profile == null) return "Reply";
            return $"Reply to {post?.Profile?.Username}";
        }
        private FormField FormFieldEditContent(ContentPostDocument? post)
        {
            return new FormField()
            {
                Name = "Content",
                //FieldType = FormFieldTypes.wysiwyg,
                Event = "form:input:user",
                Placeholder = "Comment..",
                ClearOnSubmit = true,
                AriaInvalid = false,
                //Max = post?.Settings?.CharLimit ?? 2048,
                Value = post?.Content,
            };
        }
        private FormField FormFieldContentWithParentSettings(ContentPostDocument? post)
        {
            return new FormField()
            {
                Name = "Content",
                //FieldType = FormFieldTypes.wysiwyg,
                Event = "form:input:user",
                Placeholder = "Comment..",
                ClearOnSubmit = true,
                AriaInvalid = false,
                //Max = post?.Settings?.CharLimit ?? 2048,
            };
        }
        private FormField FormFieldLink(ContentPostDocument? post)
        {
            return new FormField()
            {
                Name = "LinkValue",
                //FieldType = FormFieldTypes.link,
                Placeholder = "Post a reply",
                ClearOnSubmit = true,
                AriaInvalid = false,
                Hidden = true,
                Disabled = true
            };
        }
        private FormField FormFieldImages(ContentPostDocument post)
        {
            return new FormField()
            {
                Name = "Images",
                FieldType = FormFieldTypes.file,
                Placeholder = "Upload image",
                Multiple = true,
                Max = 4,
                ClearOnSubmit = true,
                Icon = "photo_camera",
                AriaInvalid = false,
                Hidden = true,
            };
        }
        private FormField FormFieldVideos(ContentPostDocument post)
        {
            return new FormField()
            {
                Name = "Videos",
                FieldType = FormFieldTypes.file,
                Placeholder = "Upload video",
                ClearOnSubmit = true,
                Multiple = true,
                Icon = "videocam",
                Max = 4,
                Accept = ".mp4,.mov",
                AriaInvalid = false,
                Hidden = true,
            };
        }
        private FormField FormFieldTags(ContentPostDocument post)
        {
            return new FormField()
            {
                Name = "Tags",
                //FieldType = FormFieldTypes.chips,
                Placeholder = "Add a tag",
                ClearOnSubmit = false,
                Multiple = true,
                AriaInvalid = false,
                Hidden = true,
                //Value = post?.Tags
            };
        }
        private FormField FormFieldCategory(ContentPostDocument post)
        {
            return new FormField()
            {
                Name = "Category",
                FieldType = FormFieldTypes.input,
                Placeholder = "Category",
                ClearOnSubmit = false,
                AriaInvalid = false,
                Hidden = true,
                //Value = post?.Category
            };
        }
        private FormField FormFieldUser(Guid userId)
        {
            return new FormField()
            {
                Name = "UserId",
                FieldType = FormFieldTypes.input,
                Hidden = true,
                Disabled = true,
                AriaInvalid = false,
                ClearOnSubmit = false,
                Value = userId,
            };
        }
        private FormField FormFieldParents(List<Guid> parentIds)
        {
            return new FormField()
            {
                Name = "ParentIds",
                FieldType = FormFieldTypes.input,
                Hidden = true,
                Disabled = true,
                AriaInvalid = false,
                ClearOnSubmit = false,
                Value = string.Join(',', parentIds),
            };
        }
        private FormField FormFieldParent(ContentPostDocument post)
        {
            return new FormField()
            {
                Name = "ParentId",
                FieldType = FormFieldTypes.input,
                Hidden = true,
                Disabled = true,
                AriaInvalid = false,
                ClearOnSubmit = false,
                Value = post?.Id,
            };
        }
        private FormField FormFieldType(ContentPostType type)
        {
            return new FormField()
            {
                Name = "Type",
                FieldType = FormFieldTypes.input,
                Hidden = true,
                Disabled = true,
                AriaInvalid = false,
                Value = type,
            };
        }
        private FormField FormFieldChannel(Guid? channelId)
        {
            return new FormField()
            {
                Name = "ChannelId",
                FieldType = FormFieldTypes.input,
                Hidden = true,
                Disabled = true,
                AriaInvalid = false,
                Value = channelId,
            };
        }
        private FormField FormFieldStatus(ContentPostDocument post)
        {
            return new FormField()
            {
                Name = "Status",
                FieldType = FormFieldTypes.input,
                Hidden = true,
                Disabled = true,
                AriaInvalid = false,
                Value = post?.Status,
            };
        }
        private FormField FormFieldStatusSelect()
        {
            return new FormField()
            {
                Name = "Status",
                FieldType = FormFieldTypes.input,
                Hidden = true,
                Disabled = true,
                AriaInvalid = false,
                Items = new List<object>()
                {
                    Enum.GetName(ContentPostEntityStatus.Private),
                    Enum.GetName(ContentPostEntityStatus.Public)
                },
                Value = Enum.GetName(ContentPostEntityStatus.Public),
            };
        }

        private FormField FormFieldLabel()
        {
            return new FormField()
            {
                Name = "Label",
                FieldType = FormFieldTypes.input,
                Placeholder = "Label",
                AriaInvalid = false,
            };
        }

        private FormField FormFieldReason()
        {
            return new FormField()
            {
                Name = "Reason",
                FieldType = FormFieldTypes.textarea,
                Placeholder = "Reason for label (optional)",
                AriaInvalid = false
            };
        }
        private FormField FormFieldContentPost(Guid id)
        {
            return new FormField()
            {
                Name = "ContentPostId",
                Class = "flat",
                Placeholder = "Quotes",
                Hidden = true,
                Disabled = true,
                Value = id,
                Helper = "",
            };
        }
        #endregion 
    }
}

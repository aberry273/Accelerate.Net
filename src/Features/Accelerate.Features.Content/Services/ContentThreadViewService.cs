using Accelerate.Features.Content.Controllers;
using Accelerate.Features.Content.Models.UI;
using Accelerate.Features.Content.Models.Views;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Helpers;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Content.Models.View;
using Accelerate.Foundations.Integrations.Elastic.Models;
using Elastic.Clients.Elasticsearch;
using ImageMagick;
using System;
using System.Collections.Generic;
using System.Threading.Channels;

namespace Accelerate.Features.Content.Services
{
    public class ContentThreadViewService : IContentThreadViewService
    {
        IMetaContentService _metaContentService;
        IContentViewSearchService _viewSearchService;
        public ContentThreadViewService(IMetaContentService metaContent, IContentViewSearchService viewSearchService)
        {
            _metaContentService = metaContent;
            _viewSearchService = viewSearchService;
        }

        private ContentBasePage CreateBaseContent(AccountUser user)
        {
            var profile = Accelerate.Foundations.Account.Helpers.AccountHelpers.CreateUserProfile(user);
            var baseModel = _metaContentService.CreatePageBaseContent(profile);
            var viewModel = new ContentBasePage(baseModel);
           
            return viewModel;
        }
        public ContentPage CreateThreadsPage(AccountUser user, SearchResponse<ContentPostDocument> posts, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var model = CreateBaseContent(user);
            var viewModel = new ContentPage(model);

            var sectionName = "Thread";
            var pageName = "All";
            viewModel.SideNavigation.Selected = $"{sectionName}s";

            var links = CreatePageNavigationGroup(sectionName, sectionName);
            links.Items.AddRange(GetThreadLinks(posts));
            viewModel.PageLinks = new List<NavigationGroup>()
            {
                links
            };

            viewModel.PageActions = CreatePageActionsGroup(sectionName, pageName);

            viewModel.Filters = _viewSearchService.CreateNavigationFilters(aggregateResponse);
            viewModel.UserId = user != null ? user.Id : null;
            viewModel.FormCreatePost = user != null ? CreatePostForm(user) : null;
            //viewModel.ModalCreateLabel = CreateModalChannelForm(user);
            //viewModel.ModalEditReply = CreateModalEditReplyForm(user);
            //viewModel.ModalDeleteReply = CreateModalDeleteReplyForm(user);
            viewModel.ActionsApiUrl = "/api/contentpostactions";
            viewModel.PostsApiUrl = "/api/contentsearch/posts";
            viewModel.FilterEvent = "filter:update";
            viewModel.ActionEvent = "action:post";
            return viewModel;
        } 
        public ContentCreatePage CreateThreadCreatePage(AccountUser user, SearchResponse<ContentPostDocument> posts)
        {
            var model = CreateBaseContent(user);
            var viewModel = new ContentCreatePage(model);
            var sectionName = "Thread";
            var pageName = $"Create{sectionName}";
            viewModel.RedirectRoute = $"/{sectionName}s";

            viewModel.SideNavigation.Selected = $"{sectionName}s";

            var links = CreatePageNavigationGroup(sectionName, sectionName);
            links.Items.AddRange(GetThreadLinks(posts));
            viewModel.PageLinks = new List<NavigationGroup>()
            {
                links
            };

            viewModel.PageActions = CreatePageActionsGroup(sectionName, pageName);

            viewModel.UserId = user != null ? user.Id : null;
            viewModel.Form = CreatePostForm(user);
            return viewModel;
        }
        public ContentCreatePage CreateThreadEditPage(AccountUser user, SearchResponse<ContentPostDocument> items, ContentPostViewDocument item)
        {
            var model = CreateBaseContent(user);
            var viewModel = new ContentCreatePage(model);
            var sectionName = "Thread";
            var pageName = $"Create{sectionName}";
            viewModel.RedirectRoute = $"/{sectionName}s";

            viewModel.SideNavigation.Selected = $"{sectionName}s";

            var links = CreatePageNavigationGroup(sectionName, sectionName);
            links.Items.AddRange(GetThreadLinks(items));
            viewModel.PageLinks = new List<NavigationGroup>()
            {
                links
            };

            viewModel.PageActions = CreatePageActionsGroup(sectionName, pageName);

            viewModel.UserId = user != null ? user.Id : null;
            viewModel.Form = CreatePostForm(user, PostbackType.PUT, item);
            return viewModel;
        }
        private NavigationGroup CreatePageNavigationGroup(string entity, string selected)
        {
            var plural = $"{entity}s";
            return new NavigationGroup()
            {
                Title = "All",
                Selected = selected,
                Items = new List<NavigationItem>()
                {
                    new NavigationItem()
                    {
                        Text = $"Create {entity}",
                        Href = $"/Content/{entity}/Create",
                        Class = "relative flex cursor-pointer select-none items-center rounded px-2 py-1.5 text-sm text-gray-500 hover:text-gray-900 hover:bg-gray-200 dark:text-gray-400 dark:hover:text-white dark:hover:bg-gray-600 data-[disabled]:pointer-events-none data-[disabled]:opacity-50"
                    },
                    new NavigationItem()
                    {
                        Text = "All",
                        Href = $"/{plural}",
                    }
                }
            };
        }
        private ButtonGroup CreatePageActionsGroup(string entity, string name)
        {
            var plural = $"{entity}s";
            return new ButtonGroup()
            {
                Title = name,
                Items = new List<ButtonItem>()
                {
                }
            };
        }
        private ButtonGroup CreatePageActionsGroup(string entity, string name, EntityDocument channel)
        {
            var plural = $"{entity}s";
            return new ButtonGroup()
            {
                Title = name,
                Items = new List<ButtonItem>()
                {
                    new ButtonItem()
                    {
                        Text = $"Edit",
                        Href = $"/Content/{entity}/Edit/{channel.Id}",
                        Class = "relative flex cursor-pointer select-none items-center rounded px-2 py-1.5 text-sm text-gray-500 hover:text-gray-900 hover:bg-gray-200 dark:text-gray-400 dark:hover:text-white dark:hover:bg-gray-600 data-[disabled]:pointer-events-none data-[disabled]:opacity-50",
                        Icon = "edit"
                    },
                    new ButtonItem()
                    {
                        Text = $"Delete",
                        Href = $"/Content/{entity}/Delete/{channel.Id}",
                        Class = "relative flex cursor-pointer select-none items-center rounded px-2 py-1.5 text-sm text-gray-500 hover:text-gray-900 hover:bg-gray-200 dark:text-gray-400 dark:hover:text-white dark:hover:bg-gray-600 data-[disabled]:pointer-events-none data-[disabled]:opacity-50",
                        Icon = "trash"
                    },
                }
            };
        }

        public ContentPostPage CreateThreadPage(AccountUser user, ContentPostViewDocument item, SearchResponse<ContentPostDocument> aggregateResponse, ContentChannelDocument? channel = null)
        {
            var model = CreateBaseContent(user);
            var viewModel = new ContentPostPage(model);
            viewModel.Item = item;
            #pragma warning restore CS8601 // Possible null reference assignment.
            viewModel.ChannelLink = GetThreadLink(item);
            if (user != null)
            {
                viewModel.UserId = user.Id;
                //viewModel.FormCreateReply = CreateReplyForm(user, item);
                viewModel.ModalCreateReply = CreateModalCreateReplyForm(user, item);
                viewModel.ModalDeleteReply = CreateModalDeleteReplyForm(user);
                viewModel.ModalLabelReply = CreateModalCreateLabelForm(user);
                viewModel.ModalPinReply = CreateModalCreatePinForm(user, item);
            }

            viewModel.ActionsApiUrl = "/api/contentpostactions";
            viewModel.PostsApiUrl = $"/api/contentsearch/posts/replies";
            viewModel.PinnedPostsApiUrl = $"/api/contentsearch/posts/pinned/{item.Id}";
          
            // Add filters
            viewModel.Filters = _viewSearchService.CreateNavigationFilters(aggregateResponse);
            return viewModel;
        }
        public ThreadEditPage CreateEditThreadPage(AccountUser user, ContentPostViewDocument item, SearchResponse<ContentPostDocument> aggregateResponse, ContentChannelDocument? channel = null)
        {
            var model = CreateBaseContent(user);
            var viewModel = new ThreadEditPage(model);
            viewModel.Item = item;
            #pragma warning restore CS8601 // Possible null reference assignment.
            viewModel.ChannelLink = GetThreadLink(item);
            if (user != null)
            {
                viewModel.UserId = user.Id;
                viewModel.EditForm = CreateEditPostForm(user, item);
                viewModel.ModalDeleteReply = CreateModalDeleteReplyForm(user);
                viewModel.ModalLabelReply = CreateModalCreateLabelForm(user);
                viewModel.ModalPinReply = CreateModalCreatePinForm(user, item);
            }

            viewModel.ActionsApiUrl = "/api/contentpostactions";
            viewModel.PostsApiUrl = $"/api/contentsearch/posts/replies";
            viewModel.PinnedPostsApiUrl = $"/api/contentsearch/posts/pinned/{item.Id}";

            // Add filters
            viewModel.Filters = _viewSearchService.CreateNavigationFilters(aggregateResponse);
            return viewModel;
        }
        public ContentBasePage CreateAnonymousListingPage()
        {
            var model = CreateBaseContent(null);
            var viewModel = new ContentBasePage(model);

            viewModel.UserId = null;
            return viewModel;
        }
        public NotFoundPage CreateNotFoundPage(AccountUser user, string title, string description)
        {
            var model = CreateBaseContent(user);
            var viewModel = new NotFoundPage(model);
            viewModel.ReturnLink = GetReturnLink();
            viewModel.Title = "Post not found";
            viewModel.Description = "We are unable to retrieve this post, this may have been deleted or made private.";
            return viewModel;
        }
        public ContentSubmitForm CreatePostForm(AccountUser user, PostbackType type = PostbackType.POST, ContentPostViewDocument item = null, ContentChannelDocument channel = null)
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
                    FormFieldContentFormats(item),
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
            if(channel != null )
            {
                model.Fields.Add(FormFieldChannel(channel.Id));
            }
            if(item != null)
            {
                model.Fields.Add(FormFieldId(item.Id));
            }
            return model;
        }
        private string GetContentPostReplyValue(ContentPostViewDocument post)
        {
            if (post?.Profile == null) return "Reply";
            return $"Reply to {post?.Profile?.Username}";
        }
        public ContentSubmitForm CreateReplyForm(AccountUser user, ContentPostViewDocument post)
        {
            //var parentIdThread = post.Related.Parents != null ? post.Related.Parents : new List<Guid>();
            //parentIdThread.Add(post.Id);
            var model = new ContentSubmitForm()
            {
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
                    FormFieldContentFormats(post, "Formats", "acl-reply-field"),
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
        public ContentSubmitForm CreateEditPostForm(AccountUser user, ContentPostDocument post)
        {
            var parentIdThread = post.Related.ParentIds != null ? post.Related.ParentIds : new List<Guid>();
            parentIdThread.Add(post.Id);
            var model = new ContentSubmitForm()
            {
                UserId = user.Id,
                SearchUsersUrl = "/api/accountsearch/users",
                FetchMetadataUrl = "/api/contentpost/metadata",
                Action = $"/api/contentpost/mixed",
                Type = PostbackType.PUT,
                Event = "post:created",
                ActionEvent = "action:post",
                Label = "Reply",
                Fields = new List<FormField>()
                {
                    FormFieldId(post.Id),
                    FormFieldMentions(post),
                    FormFieldQuotes(post),
                    FormFieldCharLimit(post),
                    FormFieldImageLimit(post),
                    FormFieldVideoLimit(post),
                    FormFieldEditContent(post),
                    /*
                    FormFieldLink(post),
                    FormFieldImages(post),
                    FormFieldVideos(post),
                    FormFieldTags(post),
                    FormFieldCategory(post),
                    FormFieldUser(user.Id),
                    FormFieldParents(parentIdThread),
                    FormFieldParent(post),
                    FormFieldChannel(post?.ChannelId),
                    */
                    FormFieldStatus(post),
                }
            };
            /*
            if(post.ParentId != null)
            {
                model.Fields.Insert(0, FormFieldReplyTo(post));
                model.Fields.Insert(0, FormFieldType(ContentPostType.Reply));
            }
            else
            {
                model.Fields.Insert(0, FormFieldType(ContentPostType.Post));
            }
            */
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
                Placeholder = this.GetContentPostReplyValue(post),
                ClearOnSubmit = true,
                AriaInvalid = false,
                //Max = post?.Settings?.CharLimit ?? 512,
            };
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
        public ModalForm CreateModalCreatePinForm(AccountUser user, ContentPostDocument post)
        {
            var model = new ModalForm();
            model.Title = "Pin post";
            model.Text = "Test form text";
            model.Event = "on:pin:create";
            model.Form = CreateFormAddPin(user, post);
            return model;
        }
        public AjaxForm CreateFormAddPin(AccountUser user, ContentPostDocument post)
        {
            var model = new AjaxForm()
            {
                Action = "/api/contentpostpin/post",
                Type = PostbackType.POST,
                Event = "post:pin:modal",
                Label = "Comment",
                Fields = new List<FormField>()
                {
                    FormFieldUser(user.Id),
                    FormFieldContentPost(post.Id),
                    FormFieldReason(),
                }
            };
            return model;

        }

        public ModalForm CreateModalCreateLabelForm(AccountUser user)
        {
            var model = new ModalForm();
            model.Title = "Label post";
            model.Text = "Test form text";
            model.Event = "on:comment:label";
            model.Form = CreateFormAddLabel(user);
            return model;
        }

        public AjaxForm CreateFormAddLabel(AccountUser user)
        {
            var model = new AjaxForm()
            {
                Action = "/api/contentpostlabel/post",
                Type = PostbackType.POST,
                Event = "post:label:modal",
                Label = "Comment",
                Fields = new List<FormField>()
                {
                    //ContentPostId is replaced by route {id} value
                    this.FormFieldContentPost(Guid.Empty),
                    this.FormFieldUser(user.Id),
                    FormFieldLabel(),
                    FormFieldReason(),
                }
            };
            return model;

        }
        public ModalCreateContentPostReply CreateModalCreateReplyForm(AccountUser user, ContentPostViewDocument post)
        {
            var model = new ModalCreateContentPostReply();
            model.Title = "Reply to post";
            model.Text = "Test form text";
            model.Event = "on:comment:reply";
            model.Component = "aclSocialFormPost";
            model.Form = new SocialPostFormModel()
            {
                Form = CreateReplyForm(user, post),
                Actions = new List<object>(),
                FormatActions = new List<string>()
            };
            return model;
        } 
        public ModalForm EditModalReplyForm(AccountUser user)
        {
            var model = new ModalForm();
            model.Title = "Edit post";
            model.Text = "Test form text";
            model.Event = "modal-edit-post";
            model.Form = CreateFormEditReply(user);
            return model;
        }
        public AjaxForm CreateFormEditReply(AccountUser user)
        {
            var model = new AjaxForm()
            {
                Action = "/api/contentpost",
                Type = PostbackType.PUT,
                Event = "post:edited:modal",
                Label = "Comment",
                Fields = new List<FormField>()
                {
                    this.FormFieldQuotes(null),
                    this.FormFieldEditContent(null),
                    /*new FormField()
                    {
                        Name = "Content",
                        FieldType = FormFieldTypes.textarea,
                        Placeholder = "Comment",
                        AriaInvalid = false
                    },
                    */
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
            model.Event = "on:comment:delete";
            model.Form = CreateFormDeleteReply(user);
            return model;
        }
        public AjaxForm CreateFormDeleteReply(AccountUser user)
        {
            var model = new AjaxForm()
            {
                Action = "/api/contentpost",
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


        // NAVIGATION 
        public List<NavigationItem> GetChannelsDropdown(ContentChannelDocument item)
        {
            return new List<NavigationItem>()
            {
                /*
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
        } 
        public List<NavigationItem> GetThreadLinks(SearchResponse<ContentPostDocument> searchResponse = null, string selectedName = null)
        {
            var model = new List<NavigationItem>();
            if (searchResponse != null && searchResponse.IsValidResponse)
            {
                var channelItems = searchResponse.Documents.Select(GetThreadLink);
                model.AddRange(channelItems);
            }
            return model;
        }

        public NavigationItem? GetThreadLink(ContentPostDocument post)
        {
            if (post == null) { return null; }
            return new NavigationItem()
            {
                Text = post.Id.ToString(),
                Href = this._metaContentService.GetActionUrl(nameof(PostsController.Index), ControllerHelper.NameOf<PostsController>(), new { id = post.Id })
            };
        }
        public NavigationItem? GetThreadLink(Guid? parentId)
        {
            if(parentId  == null) { return null; }
            return new NavigationItem()
            {
                Text = $"Return to parent",
                Href = this._metaContentService.GetActionUrl(nameof(PostsController.Index), ControllerHelper.NameOf<PostsController>(), new { id = parentId })
            };
        } 
        public NavigationItem? GetReturnLink()
        { 
            return new NavigationItem()
            {
                Text = "Return",
                Href = this._metaContentService.GetActionUrl(nameof(PostsController.Index), ControllerHelper.NameOf<PostsController>(), new { })
            };
        } 

    }
}

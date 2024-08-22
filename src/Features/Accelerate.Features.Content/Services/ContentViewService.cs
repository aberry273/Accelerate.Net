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
using Elastic.Clients.Elasticsearch.Core.TermVectors;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Drawing;
using System.Security.Principal;
using System.Threading.Channels;

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
            var profile = Accelerate.Foundations.Account.Helpers.AccountHelpers.CreateUserProfile(user);
            var model = _metaContentService.CreatePageBaseContent(profile);
            return model;
        }
        public ChannelsPage CreateAnonymousChannelsPage()
        {
            var model = CreateBaseContent(null);
            var viewModel = new ChannelsPage(model);
            viewModel.ChannelsTabs = GetChannelsTabs();

            viewModel.UserId = null;
            return viewModel;
        }
        public ChannelsPage CreateChannelsPage(AccountUser user, SearchResponse<ContentChannelDocument> channels, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var model = CreateBaseContent(user);
            var viewModel = new ChannelsPage(model);
            viewModel.ChannelsTabs = GetChannelsTabs(channels);
            /*
            if (channels != null && channels.IsValidResponse)
            {
                var channelItems = channels.Documents.Select(x => new NavigationItem()
                {
                    Text = x.Name,
                    Href = this._metaContentService.GetActionUrl(nameof(ChannelsController.Channel), ControllerHelper.NameOf<ChannelsController>(), new { id = x.Id })
                });
                viewModel.ChannelsDropdown.Items.AddRange(channelItems);
            }
            */

            viewModel.Filters = CreateNavigationFilters(aggregateResponse);
            viewModel.UserId = user != null ? user.Id : null;
            viewModel.FormCreatePost = user != null ? CreatePostForm(user) : null;
            viewModel.ModalCreateChannel = CreateModalChannelForm(user);
            viewModel.ModalCreateLabel = CreateModalChannelForm(user);
            //viewModel.ModalEditReply = CreateModalEditReplyForm(user);
            viewModel.ModalDeleteReply = CreateModalDeleteReplyForm(user);
            viewModel.ActionsApiUrl = "/api/contentpostactions";
            viewModel.PostsApiUrl = "/api/contentsearch/posts";
            viewModel.FilterEvent = "filter:update";
            viewModel.ActionEvent = "action:post";
            return viewModel;
        }
        public ChannelPage CreateChannelPage(AccountUser user, ContentChannelDocument item, SearchResponse<ContentChannelDocument> channels, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var model = CreateBaseContent(user);
            var viewModel = new ChannelPage(model);

            viewModel.Item = item;
            viewModel.ChannelsTabs = GetChannelsTabs(channels);
            viewModel.ChannelDropdown = GetChannelsDropdown(item);

            // Add filters
            viewModel.Filters = CreateNavigationFilters(aggregateResponse);

            viewModel.UserId = user.Id;
            viewModel.FormCreateReply = CreatePostForm(user, item);
            viewModel.ModalEditChannel = EditModalChannelForm(user, item);
            viewModel.ModalDeleteChannel = CreateModalDeleteChannelForm(user, item);
            //viewModel.ModalEditReply = CreateModalEditReplyForm(user);
            //viewModel.ModalDeleteReply = CreateModalDeleteReplyForm(user);
            viewModel.ActionsApiUrl = "/api/contentpostactions";
            viewModel.PostsApiUrl = "/api/contentsearch/posts";
            viewModel.FilterEvent = "filter:update";
            viewModel.ActionEvent = "action:post";
            return viewModel;
        }
        public ThreadPage CreateThreadPage(AccountUser user, ContentPostDocument item, SearchResponse<ContentPostDocument> aggregateResponse, ContentChannelDocument? channel = null)
        {
            var model = CreateBaseContent(user);
            var viewModel = new ThreadPage(model);
            viewModel.Item = item;
            #pragma warning restore CS8601 // Possible null reference assignment.
            viewModel.ChannelLink = GetChannelLink(channel);
            if(user != null)
            {
                viewModel.UserId = user.Id;
                viewModel.FormCreateReply = CreateReplyForm(user, item);
                viewModel.ModalEditReply = CreateModalEditReplyForm(user);
                viewModel.ModalDeleteReply = CreateModalDeleteReplyForm(user);
                viewModel.ModalLabelReply = CreateModalCreateLabelForm(user);
                viewModel.ModalPinReply = CreateModalCreatePinForm(user, item);
            }

            viewModel.ActionsApiUrl = "/api/contentpostactions";
            viewModel.PostsApiUrl = $"/api/contentsearch/posts/replies";
            viewModel.PinnedPostsApiUrl = $"/api/contentsearch/posts/pinned/{item.Id}";
          
            // Add filters
            viewModel.Filters = CreateNavigationFilters(aggregateResponse);
            return viewModel;
        }
        public ThreadEditPage CreateEditThreadPage(AccountUser user, ContentPostDocument item, SearchResponse<ContentPostDocument> aggregateResponse, ContentChannelDocument? channel = null)
        {
            var model = CreateBaseContent(user);
            var viewModel = new ThreadEditPage(model);
            viewModel.Item = item;
            #pragma warning restore CS8601 // Possible null reference assignment.
            viewModel.ChannelLink = GetChannelLink(channel);
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
            viewModel.Filters = CreateNavigationFilters(aggregateResponse);
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
        public ContentSubmitForm CreatePostForm(AccountUser user, ContentChannelDocument channel = null)
        {
            var model = new ContentSubmitForm()
            {
                //FixTop = true,
                UserId = user.Id,
                SearchUsersUrl = "/api/accountsearch/users",
                FetchMetadataUrl = "/api/contentpost/metadata",
                PostbackUrl = "/api/contentpost/mixed",
                Type = PostbackType.POST,
                Event = "post:created",
                ActionEvent = "action:post",
                Label = "Submit",
                Fields = new List<FormField>()
                {
                    FormFieldMentions(null),
                    FormFieldQuotes(null),
                    FormFieldCharLimit(null),
                    FormFieldImageLimit(null),
                    FormFieldVideoLimit(null),
                    FormFieldContentBasic(null),
                    FormFieldStatusSelect(),
                    FormFieldLink(null),
                    FormFieldImages(null),
                    FormFieldVideos(null),
                    FormFieldTags(null),
                    FormFieldCategory(null),
                    FormFieldUser(user.Id),
                    FormFieldType(ContentPostType.Post),
                }
            };
            if(channel != null )
            {
                model.Fields.Add(FormFieldChannel(channel.Id));
            }
            return model;
        }
        private string GetContentPostReplyValue(ContentPostDocument post)
        {
            var content = string.Empty;
            if (post == null || string.IsNullOrEmpty(post.Content))
            {
                if (post.Media.Count() > 0)
                {
                    content = $"<{post.Media.Count()} Media>";
                }
                else if (post.QuotedPosts.Count() > 0)
                {
                    content = $"<{post.QuotedPosts.Count()} Quote>";
                }
            }
            else
            {
                content = post.Content.Length > 64 ? post.Content.Substring(0, 64) + "..." : post.Content;
            }
            return $"Reply to @{post?.Profile?.Username}: {content} #{post.ShortThreadId}";
        }
        public ContentSubmitForm CreateReplyForm(AccountUser user, ContentPostDocument post)
        {
            var parentIdThread = post.ParentIds != null ? post.ParentIds : new List<Guid>();
            parentIdThread.Add(post.Id);
            var model = new ContentSubmitForm()
            {
                UserId = user.Id,
                SearchUsersUrl = "/api/accountsearch/users",
                FetchMetadataUrl = "/api/contentpost/metadata",
                PostbackUrl = "/api/contentpost/mixed",
                Type = PostbackType.POST,
                Event = "post:created",
                ActionEvent = "action:post",
                Label = "Reply",
                Fields = new List<FormField>()
                {
                    FormFieldReplyTo(post),
                    FormFieldMentions(post),
                    FormFieldQuotes(post),
                    FormFieldCharLimit(post),
                    FormFieldImageLimit(post),
                    FormFieldVideoLimit(post),
                    FormFieldContentWithParentSettings(post),
                    FormFieldLink(post),
                    FormFieldImages(post),
                    FormFieldVideos(post),
                    FormFieldTags(post),
                    FormFieldCategory(post),
                    FormFieldUser(user.Id),
                    FormFieldParents(parentIdThread),
                    FormFieldParent(post),
                    FormFieldType(ContentPostType.Reply),
                    FormFieldChannel(post?.ChannelId),
                    FormFieldStatus(post),
                }
            }; 
            return model;
        }
        public ContentSubmitForm CreateEditPostForm(AccountUser user, ContentPostDocument post)
        {
            var parentIdThread = post.ParentIds != null ? post.ParentIds : new List<Guid>();
            parentIdThread.Add(post.Id);
            var model = new ContentSubmitForm()
            {
                UserId = user.Id,
                SearchUsersUrl = "/api/accountsearch/users",
                FetchMetadataUrl = "/api/contentpost/metadata",
                PostbackUrl = $"/api/contentpost/mixed",
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
            if(post.ParentId != null)
            {
                model.Fields.Insert(0, FormFieldReplyTo(post));
                model.Fields.Insert(0, FormFieldType(ContentPostType.Reply));
            }
            else
            {
                model.Fields.Insert(0, FormFieldType(ContentPostType.Post));
            }
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
        private FormField FormFieldReplyTo(ContentPostDocument post)
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
                FieldType = FormFieldTypes.quotes,
                Class = "flat",
                Placeholder = "Quotes",
                IsArray = true,
                Multiple = true,
                Autocomplete = null,
                ClearOnSubmit = true,
                AriaInvalid = true,
                Hidden = false,
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
        private FormField FormFieldContentBasic(ContentPostDocument post)
        {
            return new FormField()
            {
                Name = "Content",
                FieldType = FormFieldTypes.basicWysiwyg,
                Event = "form:input:user",
                Placeholder = "Comment..",
                ClearOnSubmit = true,
                AriaInvalid = false,
                Max = post?.Settings?.CharLimit ?? 512,
            };
        }
        private FormField FormFieldEditContent(ContentPostDocument? post)
        {
            return new FormField()
            {
                Name = "Content",
                FieldType = FormFieldTypes.wysiwyg,
                Event = "form:input:user",
                Placeholder = "Comment..",
                ClearOnSubmit = true,
                AriaInvalid = false,
                Max = post?.Settings?.CharLimit ?? 2048,
                Value = post?.Content,
            };
        }
        private FormField FormFieldContentWithParentSettings(ContentPostDocument? post)
        {
            return new FormField()
            {
                Name = "Content",
                FieldType = FormFieldTypes.wysiwyg,
                Event = "form:input:user",
                Placeholder = "Comment..",
                ClearOnSubmit = true,
                AriaInvalid = false,
                Max = post?.Settings?.CharLimit ?? 2048,
            };
        }
        private FormField FormFieldLink(ContentPostDocument? post)
        {
            return new FormField()
            {
                Name = "LinkValue",
                FieldType = FormFieldTypes.link,
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
                FieldType = FormFieldTypes.chips,
                Placeholder = "Add a tag",
                ClearOnSubmit = false,
                Multiple = true,
                AriaInvalid = false,
                Hidden = true,
                Value = post?.Tags
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
                Value = post?.Category
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
                Items = new List<string>()
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
        public ModalForm EditModalChannelForm(AccountUser user, ContentChannelDocument channel)
        {
            var model = new ModalForm();
            model.Title = "Edit channel";
            model.Text = "Test form text";
            model.Target = "modal-edit-channel";
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
                        ClearOnSubmit = false,
                        AriaInvalid = false,
                        Hidden = true,
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
                        ClearOnSubmit = false,
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
        public AjaxForm CreateEditChannelForm(AccountUser user, ContentChannelDocument channel)
        {
            var model = new AjaxForm()
            {
                PostbackUrl = "/api/contentchannel",
                Type = PostbackType.PUT,
                Event = "channel:create:modal",
                Label = "Create",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "Id",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Channel name",
                        AriaInvalid = false,
                        Value = channel.Id,
                        Hidden = true
                    },
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
                        ClearOnSubmit = false,
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
        public ModalForm CreateModalCreatePinForm(AccountUser user, ContentPostDocument post)
        {
            var model = new ModalForm();
            model.Title = "Pin post";
            model.Text = "Test form text";
            model.Target = "modal-pin-post";
            model.Form = CreateFormAddPin(user, post);
            return model;
        }
        public AjaxForm CreateFormAddPin(AccountUser user, ContentPostDocument post)
        {
            var model = new AjaxForm()
            {
                PostbackUrl = "/api/contentpostpin/post",
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
            model.Target = "modal-label-post";
            model.Form = CreateFormAddLabel(user);
            return model;
        }

        public AjaxForm CreateFormAddLabel(AccountUser user)
        {
            var model = new AjaxForm()
            {
                PostbackUrl = "/api/contentpostlabel/post",
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
        public ModalForm CreateModalEditReplyForm(AccountUser user)
        {
            var model = new ModalForm();
            model.Title = "Edit post";
            model.Text = "Test form text";
            model.Target = "modal-edit-post";
            model.Form = CreateFormEditReply(user);
            return model;
        }
        public ModalForm CreateModalEditChannelForm(AccountUser user, ContentChannelDocument channel)
        {
            var model = new ModalForm();
            model.Title = "Edit post";
            model.Text = "Test form text";
            model.Target = "modal-edit-channel";
            model.Form = CreateEditChannelForm(user, channel);
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

        public ModalForm CreateModalDeleteChannelForm(AccountUser user, ContentChannelDocument channel)
        {
            var model = new ModalForm();
            model.Title = "Delete channel";
            model.Text = "Testdelete form text";
            model.Target = "modal-delete-channel";
            model.Form = CreateFormDeleteChannel(user, channel);
            return model;
        }
        public AjaxForm CreateFormDeleteChannel(AccountUser user, ContentChannelDocument channel)
        {
            var model = new AjaxForm()
            {
                PostbackUrl = "/api/contentchannel/"+channel.Id,
                Type = PostbackType.DELETE,
                Event = "channel:deleted:modal",
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
                        Value = channel.Id,
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
        
        public List<KeyValuePair<string, string>> GetFilterOptions()
        {
            return new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(Constants.Filters.Category, Foundations.Content.Constants.Fields.Category.ToCamelCase()),
                new KeyValuePair<string, string>(Constants.Filters.Labels, Foundations.Content.Constants.Fields.Labels.ToCamelCase()),
                new KeyValuePair<string, string>(Constants.Filters.Tags, Foundations.Content.Constants.Fields.Tags.ToCamelCase()),
                new KeyValuePair<string, string>(Constants.Filters.Votes, Foundations.Content.Constants.Fields.ParentVote.ToCamelCase()),
                new KeyValuePair<string, string>(Constants.Filters.Threads, Foundations.Content.Constants.Fields.ShortThreadId.ToCamelCase()),
                new KeyValuePair<string, string>(Constants.Filters.Quotes, Foundations.Content.Constants.Fields.QuoteIds.ToCamelCase()),
                new KeyValuePair<string, string>(Constants.Filters.Sort, Constants.Filters.Sort),
            };
        }

        private string GetFilterOptionKey(string filterValue)
        {
            return GetFilterOptions().FirstOrDefault(y => y.Value == filterValue).Value;
        }


        // NAVIGATION 
        private NavigationFilter CreateNavigationFilters(SearchResponse<ContentPostDocument> aggregateResponse)
        {
            return new NavigationFilter()
            {
                Filters = CreateSearchFilters(aggregateResponse),
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
        public List<NavigationFilterItem> CreateSearchFilters(SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var filterValues = new Dictionary<string, List<NavigationFilterValue>>();
            if (aggregateResponse.IsValidResponse)
            {
                var filterOptions = GetFilterOptions();
                filterValues = filterOptions
                    .Select(x => x.Value)
                    .ToDictionary(x => x, x => GetValuesFromAggregate(aggregateResponse.Aggregations, x));
            }
            return CreateNavigationFilters(filterValues);
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
        public List<QueryFilter>? GetActualFilterKeys(List<QueryFilter>? Filters)
        {
            return Filters
                ?
                .Select(x =>
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
        private List<NavigationFilterValue> GetAggregateValues(IDictionary<string, List<NavigationFilterValue>> aggFilters, string key)
        {
            if (key == null) return new List<NavigationFilterValue>();
            return aggFilters.ContainsKey(key) ? aggFilters[key] : new List<NavigationFilterValue>();
        } 
        private List<NavigationFilterItem> CreateNavigationFilters(IDictionary<string, List<NavigationFilterValue>> filters)
        { 
            if(filters == null) filters = new Dictionary<string, List<NavigationFilterValue>>();
            var filter = new List<NavigationFilterItem>();

            var Votes = GetAggregateValues(filters, GetFilterKey(Constants.Filters.Votes));
            if (Votes.Count > 0)
            {
                filter.Add(new NavigationFilterItem()
                {
                    Name = Constants.Filters.Votes,
                    FilterType = NavigationFilterType.Radio,
                    Values = Votes
                });
            }
            var Actions = GetAggregateValues(filters, GetFilterKey(Constants.Filters.Actions));
            if (Actions.Count > 0)
            {
                filter.Add(new NavigationFilterItem()
                {
                    Name = Constants.Filters.Actions,
                    FilterType = NavigationFilterType.Select,
                    Values = Actions
                });
            }
            var threads = GetAggregateValues(filters, GetFilterKey(Constants.Filters.Threads));
            if (threads.Count > 0)
            {
                filter.Add(new NavigationFilterItem()
                {
                    Name = Constants.Filters.Threads,
                    FilterType = NavigationFilterType.Checkbox,
                    Values = threads
                });
            }
            var quotes = GetAggregateValues(filters, GetFilterKey(Constants.Filters.Quotes));
            if (quotes.Count > 0)
            {
                filter.Add(new NavigationFilterItem()
                {
                    Name = Constants.Filters.Quotes,
                    FilterType = NavigationFilterType.Checkbox,
                    Values = quotes
                });
            }

            var tags = GetAggregateValues(filters, GetFilterKey(Constants.Filters.Tags));
            if (tags.Count > 0)
            {
                filter.Add(new NavigationFilterItem()
                {
                    Name = Constants.Filters.Tags,
                    FilterType = NavigationFilterType.Checkbox,
                    Values = tags
                });
            }


            var labels = GetAggregateValues(filters, GetFilterKey(Constants.Filters.Labels));
            if (labels.Count > 0)
            {
                filter.Add(new NavigationFilterItem()
                {
                    Name = Constants.Filters.Labels,
                    FilterType = NavigationFilterType.Checkbox,
                    Values = labels
                });
            }

            var content = GetAggregateValues(filters, GetFilterKey(Constants.Filters.Content));
            if (content.Count > 0)
            {
                filter.Add(new NavigationFilterItem()
                {
                    Name = Constants.Filters.Content,
                    FilterType = NavigationFilterType.Select,
                    Values = content
                });
            }
             
            return filter;
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

        public Elastic.Clients.Elasticsearch.SortOrder GetSortOrderField(List<QueryFilter>? Filters)
        {
            var sortField = Filters.FirstOrDefault(x => x.Name == Constants.Filters.SortOrder);
            if (sortField == null)
            {
                return Elastic.Clients.Elasticsearch.SortOrder.Desc;
            }
            var val = sortField.Value?.ToString();
            if (string.IsNullOrEmpty(val)) return Elastic.Clients.Elasticsearch.SortOrder.Desc; 
            if (val == "Asc") return Elastic.Clients.Elasticsearch.SortOrder.Asc;
            return Elastic.Clients.Elasticsearch.SortOrder.Desc;
        }
        public string? GetSortField(List<QueryFilter>? Filters)
        {
            var sortField = Filters.FirstOrDefault(x => x.Name == Constants.Filters.Sort);
            if(sortField == null)
            {
                return null;
            }
            var val = sortField.Value?.ToString();
            if (string.IsNullOrEmpty(val)) return null;
            var option = GetFilterSortOptions().FirstOrDefault(x => x.Key == val);
            return option.Key;
        }

        public List<NavigationItem> GetChannelsDropdown(ContentChannelDocument item)
        {
            return new List<NavigationItem>()
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
        }
        public NavigationGroup GetChannelsTabs(SearchResponse<ContentChannelDocument> searchResponse = null, string selectedName = null)
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
         
        public NavigationItem? GetThreadLink(Guid? parentId)
        {
            if(parentId  == null) { return null; }
            return new NavigationItem()
            {
                Text = $"Return to parent",
                Href = this._metaContentService.GetActionUrl(nameof(ThreadsController.Thread), ControllerHelper.NameOf<ThreadsController>(), new { id = parentId })
            };
        }
        public NavigationItem? GetChannelLink(ContentChannelDocument x)
        {
            if (x == null) { return null; }
            return new NavigationItem()
            {
                Text = $"{x.Name}",
                Href = this._metaContentService.GetActionUrl(nameof(ChannelsController.Channel), ControllerHelper.NameOf<ChannelsController>(), new {id = x.Id })
            };
        }
        public NavigationItem? GetReturnLink()
        { 
            return new NavigationItem()
            {
                Text = "Return",
                Href = this._metaContentService.GetActionUrl(nameof(ChannelsController.Index), ControllerHelper.NameOf<ChannelsController>(), new { })
            };
        }
        private string GetChannelUrl(ContentChannelDocument x)
        {
            return this._metaContentService.GetActionUrl(nameof(ChannelsController.Channel), ControllerHelper.NameOf<ChannelsController>(), new { id = x.Id });
        }

    }
}

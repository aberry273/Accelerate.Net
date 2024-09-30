using Accelerate.Features.Admin.Models.Views;
using Accelerate.Features.Admin.Services; 
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Helpers;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Content.Models.View;
using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.Integrations.Elastic.Models;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Operations.Models.Entities;
using Elastic.Clients.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Threading.Channels;

namespace Accelerate.Features.Admin.Services
{
    public class AdminBaseEntityViewService<T> : IAdminBaseEntityViewService<T> where T : IBaseEntity
    {
        IMetaContentService _metaContentService; 
        protected string ItemUrl { get; set; } 
        protected string EntityName { get; set; }
        protected string ApiUrl { get; set; }
        protected string EventDelete { get { return $"on:{this.EntityName.ToLower()}:delete"; } }
        protected string EventEdit { get { return $"on:{this.EntityName.ToLower()}:edit"; } }
        public AdminBaseEntityViewService(
            IMetaContentService metaContent  
            )
        {
            _metaContentService = metaContent; 
            this.ApiUrl = "/api/contentsearch/posts";
            //ItemUrl = this._metaContentService.GetActionUrl(nameof(FeedsController.Feed), ControllerHelper.NameOf<FeedsController>(), new { id = x.Id });
        }

        private AdminBasePage CreateBaseAdminPage(AccountUser user)
        {
            var profile = Accelerate.Foundations.Account.Helpers.AccountHelpers.CreateUserProfile(user);
            var baseModel = _metaContentService.CreatePageBaseContent(profile);
            var viewModel = new AdminBasePage(baseModel);
            return viewModel;
        }
        public NotFoundPage CreateNotFoundPage(AccountUser user, string title = null, string description = null)
        {
            var model = CreateBaseAdminPage(user);
            var viewModel = new NotFoundPage(model);
            viewModel.ReturnLink = GetReturnLink();
            viewModel.Title = title ?? "Page not found";
            viewModel.Description = description ?? "We are unable to retrieve this page, it may have been deleted or made private.";
            return viewModel;
        }

        public AdminBasePage CreateAnonymousListingPage()
        {
            var model = CreateBaseAdminPage(null);
            var viewModel = new AdminCreatePage(model);

            viewModel.UserId = null;
            return viewModel;
        }
        public virtual AdminBasePage CreateAllPage(AccountUser user, IEnumerable<T> items, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var model = CreateBaseAdminPage(user);
            var viewModel = new AdminCreatePage(model);
            var pageName = "All";
            viewModel.SideNavigation.Selected = $"{this.EntityName}s";
            viewModel.PageLinks = CreatePageNavigationGroup(this.EntityName, pageName);
            viewModel.PageLinks.Items.AddRange(GetLinks(items));
            viewModel.PageActions = CreatePageActionsGroup(this.EntityName, pageName);
             
            viewModel.UserId = user != null ? user.Id : null;
            //viewModel.FormCreatePost = user != null ? CreateForm(user) : null;
            return viewModel;
        }
        public virtual AdminBasePage CreateIndexPage(AccountUser user, IEnumerable<T> items, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var model = CreateBaseAdminPage(user);
            var viewModel = new AdminCreatePage(model); 
            var pageName = "All";
            viewModel.SideNavigation.Selected = $"{this.EntityName}s";
            viewModel.PageLinks = CreatePageNavigationGroup(this.EntityName, pageName);
            viewModel.PageLinks.Items.AddRange(GetLinks(items));
            viewModel.PageActions = CreatePageActionsGroup(this.EntityName, pageName);
             
            viewModel.UserId = user != null ? user.Id : null;
            //viewModel.FormCreatePost = user != null ? CreateForm(user) : null;
            return viewModel;
        }
        public virtual string GetEntityName(T item)
        {
            return item.Id.ToString();
        }

        public virtual AdminIndexPage<T> CreateEntityPage(AccountUser user, T item, IEnumerable<T> items, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var model = CreateBaseAdminPage(user);
            var viewModel = new AdminIndexPage<T>(model);
            viewModel.Id = item.Id;
            var pageName = GetEntityName(item);
            viewModel.SideNavigation.Selected = $"{this.EntityName}s";
            viewModel.PageLinks = CreatePageNavigationGroup(this.EntityName, pageName);
            viewModel.PageActions = CreatePageActionsGroup(this.EntityName, pageName, item);
            viewModel.PageLinks.Items.AddRange(GetLinks(items));
            viewModel.ParentUrl = $"/{this.EntityName}s";
            viewModel.PostsApiUrl = this.ApiUrl;
            viewModel.ModalDelete = CreateModalDeleteForm(user, item);
            
            viewModel.UserId = user.Id;
            viewModel.Form = CreateForm(user, item, PostbackType.PUT);
            return viewModel;
        }
        public virtual AdminCreatePage CreateAddPage(AccountUser user, IEnumerable<T> items)
        {
            var model = CreateBaseAdminPage(user);
            var viewModel = new AdminCreatePage(model); 
            var pageName = $"Create {this.EntityName}";
            viewModel.RedirectRoute = $"/{this.EntityName}s";

            viewModel.SideNavigation.Selected = $"{this.EntityName}s";
            viewModel.PageLinks = CreatePageNavigationGroup(this.EntityName, pageName);
            viewModel.PageLinks.Items.AddRange(GetLinks(items));
            viewModel.PageActions = CreatePageActionsGroup(this.EntityName, pageName);

            viewModel.UserId = user != null ? user.Id : null;
            viewModel.Form = CreateForm(user);
            return viewModel;
        } 

        public virtual AdminCreatePage CreateEditPage(AccountUser user, IEnumerable<T> items, T item)
        {
            var model = CreateBaseAdminPage(user);
            var viewModel = new AdminCreatePage(model);
            var pageName = $"Edit {this.GetEntityName(item)}";
            viewModel.RedirectRoute = $"/{this.EntityName}s";
            viewModel.SideNavigation.Selected = $"{this.EntityName}s";
            viewModel.PageLinks = CreatePageNavigationGroup(this.EntityName, item.Id.ToString());
            viewModel.PageLinks.Items.AddRange(GetLinks(items));
            viewModel.PageActions = CreatePageActionsGroup(this.EntityName, pageName);
            viewModel.ModalDelete = CreateModalDeleteForm(user, item);

            viewModel.UserId = user?.Id;
            viewModel.Form = CreateForm(user, item, PostbackType.PUT);
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
                        Href = $"/Admin/{plural}/Create",
                        Class = "relative flex cursor-pointer select-none items-center rounded px-2 py-1.5 text-sm text-gray-500 hover:text-gray-900 hover:bg-gray-200 dark:text-gray-400 dark:hover:text-white dark:hover:bg-gray-600 data-[disabled]:pointer-events-none data-[disabled]:opacity-50"
                    },
                    new NavigationItem()
                    {
                        Text = "All",
                        Href = $"/Admin/{plural}",
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
        private ButtonGroup CreatePageActionsGroup(string entity, string name, IBaseEntity item)
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
                        Href = $"/Admin/{plural}/{item.Id}/Edit",
                        Class = "relative flex cursor-pointer select-none items-center rounded px-2 py-1.5 text-sm text-gray-500 hover:text-gray-900 hover:bg-gray-200 dark:text-gray-400 dark:hover:text-white dark:hover:bg-gray-600 data-[disabled]:pointer-events-none data-[disabled]:opacity-50",
                        Icon = "edit"
                    },
                    new ButtonItem()
                    {
                        Text = $"Delete",
                        Event = EventDelete,
                        Class = "relative flex cursor-pointer select-none items-center rounded px-2 py-1.5 text-sm text-gray-500 hover:text-gray-900 hover:bg-gray-200 dark:text-gray-400 dark:hover:text-white dark:hover:bg-gray-600 data-[disabled]:pointer-events-none data-[disabled]:opacity-50",
                        Icon = "trash"
                    },
                    new ButtonItem()
                    {
                        Text = $"Search",
                        Href = $"/Admin/{plural}/{item.Id}/Search",
                        Class = "relative flex cursor-pointer select-none items-center rounded px-2 py-1.5 text-sm text-gray-500 hover:text-gray-900 hover:bg-gray-200 dark:text-gray-400 dark:hover:text-white dark:hover:bg-gray-600 data-[disabled]:pointer-events-none data-[disabled]:opacity-50",
                        Icon = "magnifyingGlass"
                    },
                }
            };
        }
        public NavigationItem? GetReturnLink()
        {
            return new NavigationItem()
            {
                Text = "Return",
                Href = "#"//this._metaContentService.GetActionUrl(nameof(ChannelsController.Index), ControllerHelper.NameOf<ChannelsController>(), new { })
            };
        } 
        private string GetContentPostReplyValue(ContentPostViewDocument post)
        {
            return $"Reply to @{post?.Profile?.Username}";
        }
        #region Generic fields
        public FormField FormFieldSelect(string name, List<KeyValuePair<string, string>> values, string value = null)
        {
            return new FormField()
            {
                Name = name,
                FieldType = FormFieldTypes.select,
                FieldComponent = FormFieldComponents.aclFieldSelect,
                Hidden = false,
                Disabled = false,
                AriaInvalid = false,
                ClearOnSubmit = false,
                AreItemsObject = true,
                Items = values.Select(x => x as object).ToList(),
                Value = value,
            };
        }
        public FormField FormFieldSelect(string name, string label, List<KeyValuePair<string, string>> values, string value = null)
        {
            return new FormField()
            {
                Name = name,
                Label = label,
                FieldType = FormFieldTypes.select,
                FieldComponent = FormFieldComponents.aclFieldSelect,
                Hidden = false,
                Disabled = false,
                AriaInvalid = false,
                ClearOnSubmit = false,
                AreItemsObject = true,
                Items = values.Select(x => x as object).ToList(),
                Value = value,
            };
        }
        public FormField FormFieldSelect(string name, List<string> values, string value = null)
        {
            return new FormField()
            {
                Name = name,
                FieldType = FormFieldTypes.select,
                FieldComponent = FormFieldComponents.aclFieldSelect,
                Hidden = false,
                Disabled = true,
                AriaInvalid = false,
                ClearOnSubmit = false,
                Items = values.Select(x => x as object).ToList(),
                Value = value,
            };
        }
        #endregion
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
        private FormField FormFieldContentFormats(ContentPostViewDocument post, string name = "Formats")
        {
            return new FormField()
            {
                Id = "aclFieldEditorJs",
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
                FieldType = FormFieldTypes.Image,
                FieldComponent = FormFieldComponents.aclFieldFile,
                Placeholder = "Upload images",
                Multiple = true,
                Max = 12,
                ClearOnSubmit = true,
                Icon = "photo_camera",
                Accept = ".png,.jpg,.jpeg",
                AriaInvalid = false,
                Hidden = true,
            };
        }
        private FormField FormFieldVideos(ContentPostDocument post)
        {
            return new FormField()
            {
                Name = "Videos",
                FieldType = FormFieldTypes.Video,
                FieldComponent = FormFieldComponents.aclFieldFile,
                Placeholder = "Upload video",
                ClearOnSubmit = true,
                Multiple = true,
                Icon = "videocam",
                Max = 12,
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
        #region Channel
        public ModalForm EditModalForm(AccountUser user, T item)
        {
            var model = new ModalForm();
            model.Title = $"Edit {this.EntityName}";
            model.Text = "Test form text";
            model.Event = EventEdit;
            model.Form = CreateForm(user, item, PostbackType.PUT);
            return model;
        } 
        public virtual AjaxForm CreateForm(AccountUser user, T? item, PostbackType type = PostbackType.POST)
        {
            var model = new AjaxForm()
            {
                Action = item == null 
                    ? $"/api/admin{this.EntityName.ToLower()}" 
                    : $"/api/admin{this.EntityName.ToLower()}/{item.Id}",
                Type = type,
                Event = $"{this.EntityName.ToLower()}:create:modal",
                Label = "Create",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "Name",
                        FieldType = FormFieldTypes.input,
                        Placeholder = $"{this.EntityName} name",
                        AriaInvalid = false,
                        Value = this.GetEntityName(item),
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
                        //Value = item?.Category,
                    },
                    new FormField()
                    {
                        Name = "TagItems",
                        Label = "Tags",
                        //FieldType = FormFieldTypes.chips,
                        Placeholder = "Listen to posts tagged with..",
                        ClearOnSubmit = false,
                        AriaInvalid = false,
                        Hidden = false,
                       // Value = item?.Tags,
                    },
                    new FormField()
                    {
                        Name = "Description",
                        FieldType = FormFieldTypes.textarea,
                        Placeholder = "Describe what content this channel is for",
                        AriaInvalid = false,
                       // Value = item?.Description,
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
            if(item != null)
            {
                model.Fields.Add(
                    new FormField()
                    {
                        Name = "Id",
                        FieldType = FormFieldTypes.input,
                        AriaInvalid = false,
                        Value = item.Id,
                        Hidden = true,
                        Disabled = true
                    }
                );
            }
            return model;
        }
        public virtual AjaxForm CreateForm(AccountUser user, PostbackType type = PostbackType.POST)
        {
            return this.CreateForm(user, type);
        }
        #endregion

        public ModalForm CreateModalDeleteForm(AccountUser user, T item)
        {
            var model = new ModalForm();
            model.Title = $"Delete {this.EntityName}";
            model.Event = EventDelete;
            model.Form = CreateFormDelete(user, item);
            return model;
        }
        public AjaxForm CreateFormDelete(AccountUser user, T item)
        {
            var model = new AjaxForm()
            {
                Action = $"/api/content{this.EntityName.ToLower()}/{item.Id}",
                Type = PostbackType.DELETE,
                Event = $"{this.EntityName.ToLower()}:deleted:modal",
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
                        Value = item.Id,
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

        public List<NavigationItem> GetLinks(IEnumerable<T> searchResponse = null, string selectedName = null)
        {
            var model = new List<NavigationItem>();
            if (searchResponse != null && searchResponse.Any())
            {
                var channelItems = searchResponse.Select(GetLink);
                model.AddRange(channelItems);
            }
            return model;
        }
        public NavigationItem? GetLink(T x)
        {
            if (x == null) { return null; }
            return new NavigationItem()
            {
                Text = $"{this.GetEntityName(x)}",
                Href = $"/Admin/{this.EntityName}s/{x.Id}",
            };
        }
    }
}

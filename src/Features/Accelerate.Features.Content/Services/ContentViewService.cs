using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace Accelerate.Features.Content.Services
{
    public class ContentViewService : IContentViewService
    {
        public AjaxForm CreatePostForm(AccountUser user)
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
                        Name = "Tags",
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

    }
}

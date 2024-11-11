using Accelerate.Features.Admin.Models.Views;
using Accelerate.Foundations.Users.Models;
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.UI.Components.Table;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Operations.Models.Entities;
using Elastic.Clients.Elasticsearch;
using System.Collections.Generic;
using Accelerate.Foundations.Portal.Services;
using Accelerate.Foundations.Accounts.Models.Entities;

namespace Accelerate.Features.Admin.Services
{
    public class AdminIndividualEntityViewService : AdminBaseEntityViewService<AccountsIndividualEntity>
    {
        protected string EventProfile { get { return $"on:{this.EntityName.ToLower()}:profile"; } }
        protected string EventModalProfile { get { return $"{this.EventProfile}:modal"; } }
        protected string EventIndex { get { return $"on:{this.EntityName.ToLower()}:index"; } }
        protected string EventModalIndex { get { return $"{this.EventIndex}:modal"; } }
        IEntityService<UsersUser> _userService;
        IEntityService<UsersProfile> _profileService;
        IElasticService<UsersUserDocument> _userSearchService;
     
        public AdminIndividualEntityViewService(
            IElasticService<UsersUserDocument> userSearchService,
            IEntityService<UsersUser> userService,
            IEntityService<UsersProfile> profileService,
            IMetaContentService metaContent,
            IPortalContentService portalContentService)
            : base(metaContent, portalContentService)
        {
            _userSearchService = userSearchService;
            _userService = userService;
            _profileService = profileService;
            EntityName = "Individual";
        }
        public override string GetEntityName(AccountsIndividualEntity item)
        {
            return item.Firstname;
        }
          
        public AjaxForm CreateActionForm(UsersUser user)
        {
            var model = base.CreateForm(user);

            model.Fields = CreateFormFields(user, null);

            return model;
        }
        public override async Task<AdminBasePage> CreateAddPage(UsersUser user, IEnumerable<AccountsIndividualEntity> items)
        {
            var model = await base.CreateAddPage(user, items);
            var viewModel = new AdminCreatePage(model);
            viewModel.Form = CreateActionForm(user);
            return viewModel;
        }

        private List<KeyValuePair<string, string>> GetActionStateItems()
        {
            var values = Enum.GetNames<UsersUserStatus>().ToList();
            var kv = values.Select(x => new KeyValuePair<string, string>(
                x,
                ((int)Enum.Parse<UsersUserStatus>(x, true)).ToString()
            )).ToList();
            return kv;
        }
        private List<string> GetUserDomains()
        {
            return new List<string>()
            {
                Foundations.Users.Constants.Domains.Public,
                Foundations.Users.Constants.Domains.Internal,
                Foundations.Users.Constants.Domains.System,
                Foundations.Users.Constants.Domains.Deactivated,
                Foundations.Users.Constants.Domains.Deleted,
            };
        }
        private string GetStateItem(AccountsIndividualEntity item)
        {
            return "State";
            //return Enum.GetName<Acc>(item?.Status ?? UsersUserStatus.Active);
        }
        public List<FormField> CreateFormFields(UsersUser user, AccountsIndividualEntity? item)
        {
            return new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "UserName",
                        FieldType = FormFieldTypes.input,
                        Placeholder = $"Name",
                        AriaInvalid = false,
                        Value = item?.Firstname,
                    },
                    new FormField()
                    {
                        Name = "Email",
                        FieldType = FormFieldTypes.email,
                        Placeholder = $"Email",
                        AriaInvalid = false,
                        Value = item?.Firstname,
                    },
                    new FormField()
                    {
                        Name = "Phone",
                        FieldType = FormFieldTypes.input,
                        Placeholder = $"Phone",
                        AriaInvalid = false,
                        Value = item?.Firstname,
                    },
                    FormFieldSelect("Domain", GetUserDomains(), item?.Firstname),
                    FormFieldSelect("State", GetActionStateItems(), GetStateItem(item)),
                };
        }
        public AjaxForm EditUserForm(UsersUser user, AccountsIndividualEntity item)
        {
            var model = base.CreateEntityForm(user, item, PostbackType.PUT);
            model.Label = $"Edit {item.Firstname}";
            model.Fields = CreateFormFields(user, item);
            return model;
        }
        public override async Task<AdminBasePage> CreateEditPage(UsersUser user, IEnumerable<AccountsIndividualEntity> items, AccountsIndividualEntity item)
        {
            var model = await base.CreateEditPage(user, items, item);
            var viewModel = new AdminCreatePage(model);
            viewModel.Form = EditUserForm(user, item);
            return viewModel;
        }
        public override async Task<AdminIndexPage<AccountsIndividualEntity>> CreateEntityPage(UsersUser user, AccountsIndividualEntity item, IEnumerable<AccountsIndividualEntity> items)
        {
            var model = await base.CreateEntityPage(user, item, items);
            var viewModel = new AdminIndexPage<AccountsIndividualEntity>(model);
            viewModel.Form = EditUserForm(user, item);
            viewModel.Form.Disabled = true;
            viewModel.Item = item; 
             
            return viewModel;
        }
         
        public ModalForm ModalIndexForm(UsersUser user, AccountsIndividualEntity item)
        {
            var model = new ModalForm();
            model.Title = $"Index {this.EntityName}";
            model.Text = "Test form text";
            model.Event = EventModalIndex;
            model.Form = CreateIndexEntityForm(user, item, PostbackType.PUT);
            return model;
        }
        public virtual AjaxForm CreateIndexEntityForm(UsersUser user, AccountsIndividualEntity? item, PostbackType type = PostbackType.POST)
        {
            var model = new AjaxForm()
            {
                Action = $"/api/admin{this.EntityName.ToLower()}/{item.Id}/index",
                Type = type,
                Event = this.EventIndex,
                Label = "Index",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "ProfileId",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        AriaInvalid = false,
                        Value = user.UsersProfileId,
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

        #region User Form


        public AjaxForm CreateProfileImageForm(UsersUser user)
        {
            var model = new AjaxForm()
            {
                Action = $"/api/adminuser/profile/{user?.UsersProfileId}/image",
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
                Action = $"/api/adminuser/deactivate",
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
                Action = $"/api/adminuser/delete/deactivated",
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
                Action = $"/api/adminuser/reactivate",
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
                Action = $"/api/adminuser/{user?.Id}",
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
                Action = $"/api/adminuser/{user.Id}/profile",
                Type = PostbackType.POST,
                Event = "on:profile:create",
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
        #endregion
    }
}

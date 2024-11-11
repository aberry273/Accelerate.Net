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
using Accelerate.Foundations.Accounts.Models;

namespace Accelerate.Features.Admin.Services
{
    public class AdminBusinessEntityViewService : AdminBaseEntityViewService<AccountsBusinessEntity>
    {
        protected string EventProfile { get { return $"on:{this.EntityName.ToLower()}:profile"; } }
        protected string EventModalProfile { get { return $"{this.EventProfile}:modal"; } }
        protected string EventIndex { get { return $"on:{this.EntityName.ToLower()}:index"; } }
        protected string EventModalIndex { get { return $"{this.EventIndex}:modal"; } }
        IEntityService<UsersUser> _userService;
        IEntityService<UsersProfile> _profileService;
        IElasticService<UsersUserDocument> _userSearchService;
     
        public AdminBusinessEntityViewService(
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
            EntityName = "Business";
            this.ApiUrl = $"/{Foundations.Common.Constants.ApiPaths.VersionPath}/Accounts/{this.EntityName}";
        }
        public override string GetEntityName(AccountsBusinessEntity item)
        {
            return item.Name;
        }
          
        public AjaxForm CreateActionForm(UsersUser user)
        {
            var model = base.CreateForm(user);

            model.Fields = CreateFormFields(user, null);

            return model;
        }
        public override async Task<AdminBasePage> CreateAddPage(UsersUser user, IEnumerable<AccountsBusinessEntity> items)
        {
            var model = await base.CreateAddPage(user, items);
            var viewModel = new AdminCreatePage(model);
            viewModel.Form = CreateActionForm(user);
            return viewModel;
        }
        public override List<object> CreateTableRows(IEnumerable<AccountsBusinessEntity> items)
        {
            return items.Select(x =>
            {
                dynamic obj = new System.Dynamic.ExpandoObject();
                obj.Name = new KeyValuePair<string, string>(x.Name, $"/Admin/{this.EntityName}/{x.Id}");
                obj.AccountType = new KeyValuePair<string, string>(Enum.GetName<BusinessAccountType>(x.AccountType), Enum.GetName<BusinessAccountType>(x.AccountType));
                obj.Website = new KeyValuePair<string, string>(x.Website, x.Website);
                obj.RegistrationAddressId = new KeyValuePair<string, string>(x.RegisteredAddressId?.ToString(), x.RegisteredAddressId?.ToString());
                obj.PrimaryContactId = new KeyValuePair<string, string>(x.PrimaryContactId.ToString(), x.PrimaryContactId.ToString());
                obj.Status = new KeyValuePair<string, string>(Enum.GetName<AccountsStatusEnum>(x.Status), Enum.GetName<AccountsStatusEnum>(x.Status));
                obj.CreatedOn = new KeyValuePair<string, string>(Foundations.Common.Extensions.DateExtensions.ToDateShort(x.CreatedOn), Foundations.Common.Extensions.DateExtensions.ToDateShort(x.CreatedOn));
                return obj;
            })?.ToList();
        }
        public override List<string> CreateTableHeaders()
        {
            return new List<string>()
            {
                "Name", "Type", "Website", "RegistrationId", "Contact", "State", "Created",
            };
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
        private string GetStateItem(AccountsBusinessEntity item)
        {
            return Enum.GetName<AccountsStatusEnum>(item.Status);
        }
        public List<FormField> CreateFormFields(UsersUser user, AccountsBusinessEntity? item)
        {
            return new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "UserName",
                        FieldType = FormFieldTypes.input,
                        Placeholder = $"Name",
                        AriaInvalid = false,
                        Value = item?.Name,
                    },
                    new FormField()
                    {
                        Name = "Email",
                        FieldType = FormFieldTypes.email,
                        Placeholder = $"Email",
                        AriaInvalid = false,
                        Value = item?.Name,
                    },
                    new FormField()
                    {
                        Name = "Phone",
                        FieldType = FormFieldTypes.input,
                        Placeholder = $"Phone",
                        AriaInvalid = false,
                        Value = item?.Name,
                    },
                    FormFieldSelect("Domain", GetUserDomains(), item?.Name),
                    FormFieldSelect("State", GetActionStateItems(), GetStateItem(item)),
                };
        }
        public AjaxForm EditUserForm(UsersUser user, AccountsBusinessEntity item)
        {
            var model = base.CreateEntityForm(user, item, PostbackType.PUT);
            model.Label = $"Edit {item.Name}";
            model.Fields = CreateFormFields(user, item);
            return model;
        }
        public override async Task<AdminBasePage> CreateEditPage(UsersUser user, IEnumerable<AccountsBusinessEntity> items, AccountsBusinessEntity item)
        {
            var model = await base.CreateEditPage(user, items, item);
            var viewModel = new AdminCreatePage(model);
            viewModel.Form = EditUserForm(user, item);
            return viewModel;
        }
        public override async Task<AdminIndexPage<AccountsBusinessEntity>> CreateEntityPage(UsersUser user, AccountsBusinessEntity item, IEnumerable<AccountsBusinessEntity> items)
        {
            var model = await base.CreateEntityPage(user, item, items);
            var viewModel = new AdminIndexPage<AccountsBusinessEntity>(model);
            viewModel.Form = EditUserForm(user, item);
            viewModel.Form.Disabled = true;
            viewModel.Item = item; 
             
            return viewModel;
        }

        public ModalForm ModalProfileForm(UsersUser user, UsersUser item)
        {
            var model = new ModalForm();
            model.Title = $"Create profile for {this.EntityName}";
            model.Text = "Test form text";
            model.Event = this.EventModalProfile;
            model.Form = CreateProfileForm(item);
            return model;
        }

        public ModalForm ModalIndexForm(UsersUser user, UsersUser item)
        {
            var model = new ModalForm();
            model.Title = $"Index {this.EntityName}";
            model.Text = "Test form text";
            model.Event = EventModalIndex;
            model.Form = CreateIndexEntityForm(user, item, PostbackType.PUT);
            return model;
        }
        public virtual AjaxForm CreateIndexEntityForm(UsersUser user, UsersUser? item, PostbackType type = PostbackType.POST)
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

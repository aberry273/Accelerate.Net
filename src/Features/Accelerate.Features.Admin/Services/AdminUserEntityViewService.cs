using Accelerate.Features.Admin.Models.Views;
using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Account.Models.Entities;
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

namespace Accelerate.Features.Admin.Services
{
    public class AdminUserEntityViewService : AdminBaseEntityViewService<AccountUser>
    {
        protected string EventProfile { get { return $"on:{this.EntityName.ToLower()}:profile"; } }
        protected string EventModalProfile { get { return $"{this.EventProfile}:modal"; } }
        protected string EventIndex { get { return $"on:{this.EntityName.ToLower()}:index"; } }
        protected string EventModalIndex { get { return $"{this.EventIndex}:modal"; } }
        IEntityService<AccountUser> _userService;
        IEntityService<AccountProfile> _profileService;
        IElasticService<AccountUserDocument> _userSearchService;
        public AdminUserEntityViewService(
            IElasticService<AccountUserDocument> userSearchService,
            IEntityService<AccountUser> userService,
            IEntityService<AccountProfile> profileService,
            IMetaContentService metaContent)
            : base(metaContent)
        {
            _userSearchService = userSearchService;
            _userService = userService;
            _profileService = profileService;
            EntityName = "User";
        }
        public override string GetEntityName(AccountUser item)
        {
            return item.UserName;
        }
          
        public AjaxForm CreateActionForm(AccountUser user)
        {
            var model = base.CreateForm(user);

            model.Fields = CreateFormFields(user, null);

            return model;
        }
        public override AdminCreatePage CreateAddPage(AccountUser user, IEnumerable<AccountUser> items)
        {
            var viewModel = base.CreateAddPage(user, items);
            viewModel.Form = CreateActionForm(user);
            return viewModel;
        }

        private List<KeyValuePair<string, string>> GetActionStateItems()
        {
            var values = Enum.GetNames<AccountUserStatus>().ToList();
            var kv = values.Select(x => new KeyValuePair<string, string>(
                x,
                ((int)Enum.Parse<AccountUserStatus>(x, true)).ToString()
            )).ToList();
            return kv;
        }
        private List<string> GetUserDomains()
        {
            return new List<string>()
            {
                Foundations.Account.Constants.Domains.Public,
                Foundations.Account.Constants.Domains.Internal,
                Foundations.Account.Constants.Domains.System,
                Foundations.Account.Constants.Domains.Deactivated,
                Foundations.Account.Constants.Domains.Deleted,
            };
        }
        private string GetStateItem(AccountUser item)
        {
            return Enum.GetName<AccountUserStatus>(item?.Status ?? AccountUserStatus.Active);
        }
        public List<FormField> CreateFormFields(AccountUser user, AccountUser? item)
        {
            return new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "UserName",
                        FieldType = FormFieldTypes.input,
                        Placeholder = $"Name",
                        AriaInvalid = false,
                        Value = item?.UserName,
                    },
                    new FormField()
                    {
                        Name = "Email",
                        FieldType = FormFieldTypes.email,
                        Placeholder = $"Email",
                        AriaInvalid = false,
                        Value = item?.Email,
                    },
                    new FormField()
                    {
                        Name = "Phone",
                        FieldType = FormFieldTypes.input,
                        Placeholder = $"Phone",
                        AriaInvalid = false,
                        Value = item?.PhoneNumber,
                    },
                    FormFieldSelect("Domain", GetUserDomains(), item?.Domain),
                    FormFieldSelect("State", GetActionStateItems(), GetStateItem(item)),
                };
        }
        public AjaxForm EditUserForm(AccountUser user, AccountUser item)
        {
            var model = base.CreateEntityForm(user, item, PostbackType.PUT);
            model.Label = $"Edit {item.UserName}";
            model.Fields = CreateFormFields(user, item);
            return model;
        }
        public override AdminCreatePage CreateEditPage(AccountUser user, IEnumerable<AccountUser> items, AccountUser item)
        {
            var viewModel = base.CreateEditPage(user, items, item);
            viewModel.Form = EditUserForm(user, item);
            return viewModel;
        }
        public override async Task<AdminIndexPage<AccountUser>> CreateEntityPage(AccountUser user, AccountUser item, IEnumerable<AccountUser> items, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var model = await base.CreateEntityPage(user, item, items, aggregateResponse);
            var viewModel = new AdminUserPage(model);
            viewModel.Form = EditUserForm(user, item);
            viewModel.Form.Disabled = true;
            viewModel.Item = item;
            var profile = _profileService.Get(item.AccountProfileId.GetValueOrDefault());
            viewModel.HasProfile = profile != null;
            item.AccountProfile = profile;
            
            viewModel.ProfileImageForm = CreateProfileImageForm(item);
            viewModel.UserForm = CreateUserForm(item);
            viewModel.ProfileForm = CreateProfileForm(item);
            viewModel.DeactivateForm = CreateDeactivateForm(item);
            viewModel.DeleteForm = CreateDeleteForm(item);
            viewModel.ReactivateForm = CreateReactivateForm(item);

            // If index not found
            var indexModel = await this._userSearchService.GetDocument<AccountUserDocument>(user.Id.ToString());

            viewModel.ModalIndex = this.ModalIndexForm(user, item);
            viewModel.PageActions.Items.Add(new Foundations.Common.Models.Views.ButtonItem()
            {
                Icon = "cog",
                Text = "Index",
                Event = this.EventModalIndex
            });
            if(profile == null)
            {
                viewModel.ModalProfile = this.ModalProfileForm(user, item);
                viewModel.PageActions.Items.Add(new Foundations.Common.Models.Views.ButtonItem()
                {
                    Icon = "user",
                    Text = "Create Profile",
                    Event = this.EventModalProfile
                });
            }

            if (indexModel.IsSuccess() && indexModel.Source == null)
            {
                viewModel.LastIndex = "N/A";
            }
            else if (indexModel.IsSuccess() && indexModel.Source != null)
            {
                viewModel.LastIndex = indexModel.Source.UpdatedOn.ToString();
            }

            return viewModel;
        }

        public ModalForm ModalProfileForm(AccountUser user, AccountUser item)
        {
            var model = new ModalForm();
            model.Title = $"Create profile for {this.EntityName}";
            model.Text = "Test form text";
            model.Event = this.EventModalProfile;
            model.Form = CreateProfileForm(item);
            return model;
        }

        public ModalForm ModalIndexForm(AccountUser user, AccountUser item)
        {
            var model = new ModalForm();
            model.Title = $"Index {this.EntityName}";
            model.Text = "Test form text";
            model.Event = EventModalIndex;
            model.Form = CreateIndexEntityForm(user, item, PostbackType.PUT);
            return model;
        }
        public virtual AjaxForm CreateIndexEntityForm(AccountUser user, AccountUser? item, PostbackType type = PostbackType.POST)
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
                        Value = user.AccountProfileId,
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


        public AjaxForm CreateProfileImageForm(AccountUser user)
        {
            var model = new AjaxForm()
            {
                Action = $"/api/adminuser/profile/{user?.AccountProfileId}/image",
                Type = PostbackType.PUT,
                Event = "profile:updated",
                IsFile = true,
                Label = "Update",
                Disabled = user.Status == AccountUserStatus.Deactivated ? true : null,
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
                        Value = user?.AccountProfile?.Image?.ToString(),
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
                        Value = user?.AccountProfileId,
                    }
                }
            };
            return model;
        }
        public AjaxForm CreateDeactivateForm(AccountUser user)
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

        public AjaxForm CreateDeleteForm(AccountUser user)
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

        public AjaxForm CreateReactivateForm(AccountUser user)
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
        public AjaxForm CreateUserForm(AccountUser user)
        {
            var model = new AjaxForm()
            {
                Action = $"/api/adminuser/{user?.Id}",
                Type = PostbackType.PUT,
                Event = "user:create",
                Disabled = user.Status == AccountUserStatus.Deactivated ? true : null,
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
                        Disabled = user.Email != user.UserName || user.Status == AccountUserStatus.Deactivated,
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
        public AjaxForm CreateProfileForm(AccountUser user)
        {
            var model = new AjaxForm()
            {
                Action = $"/api/adminuser/{user.Id}/profile",
                Type = PostbackType.POST,
                Event = "on:profile:create",
                Label = "Update",
                Disabled = user.Status == AccountUserStatus.Deactivated ? true : null,
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Label = "Firstname",
                        Name = "Firstname",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Firstname",
                        Disabled = user.Status == AccountUserStatus.Deactivated ? true : null,
                        Value = user?.AccountProfile?.Firstname,
                    },
                    new FormField()
                    {
                        Label = "Lastname",
                        Name = "Lastname",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Lastname",
                        Disabled = user.Status == AccountUserStatus.Deactivated ? true : null,
                        Value = user?.AccountProfile?.Lastname,
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
                        Value = user?.AccountProfileId,
                    }
                }
            };
            return model;
        }
        #endregion
    }
}

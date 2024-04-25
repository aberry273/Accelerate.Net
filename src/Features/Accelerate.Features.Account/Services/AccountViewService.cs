using Accelerate.Features.Account.Controllers;
using Accelerate.Features.Account.Models.Views;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Helpers;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Common.Services;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Aggregations;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Security.Policy;
using System.Threading.Channels;

namespace Accelerate.Features.Account.Services
{
    public class AccountViewService : IAccountViewService
    {

        private IMetaContentService _contentService;
        public AccountViewService(
            IMetaContentService contentService)
        {
            _contentService = contentService;
        }
        #region Manage

        private UserProfile GetUserProfile(AccountUser user)
        {
            if (user == null) return null;
            return new UserProfile()
            {
                Image = user?.AccountProfile?.Image,
                Username = user?.UserName
            };
        }
        public ManagePage GetManagePage(AccountUser user)
        {
            var viewModel = new ManagePage(_contentService.CreatePageBaseContent(GetUserProfile(user)));
            viewModel.UserId = user.Id;
            viewModel.ProfileImageForm = CreateProfileImageForm(user);
            viewModel.ProfileForm = CreateProfileForm(user);
            viewModel.ModalCreateMedia = CreateModalMediaForm(user);
            viewModel.Tabs = new List<NavigationItem>()
            {
                GetPageLink(nameof(AccountController.Profile)),
                GetPageLink(nameof(AccountController.Posts)),
                GetPageLink(nameof(AccountController.Media)),
                //GetPageLink(nameof(AccountController.Settings)),
            };
            return viewModel;
        }
        public AjaxForm CreateProfileImageForm(AccountUser user)
        {
            var model = new AjaxForm()
            {
                PostbackUrl = $"https://localhost:7220/api/accountprofile/{user?.AccountProfileId}/image",
                Type = PostbackType.PUT,
                Event = "profile:updated",
                Label = "Update",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Id = "create-profile-image-file",
                        Label = "File",
                        Name = "Image",
                        FieldType = FormFieldTypes.file,
                        Accept = ".png,.jpg",
                        Placeholder = "Image",
                        Value = user?.AccountProfile?.Image,
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
        public AjaxForm CreateProfileForm(AccountUser user)
        {
            var model = new AjaxForm()
            {
                PostbackUrl = $"https://localhost:7220/api/accountprofile/{user?.AccountProfileId}",
                Type = PostbackType.PUT,
                Event = "profile:updated",
                Label = "Update",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Label = "Firstname",
                        Name = "Firstname",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Firstname",
                        Value = user?.AccountProfile?.Firstname,
                    },
                    new FormField()
                    {
                        Label = "Lastname",
                        Name = "Lastname",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Lastname",
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
        public ModalForm CreateModalMediaForm(AccountUser user)
        {
            var model = new ModalForm();
            model.Title = "Create media";
            model.Text = "Test form text";
            model.Target = "modal-create-channel";
            model.Form = CreateMediaForm(user);
            return model;
        }
        public AjaxForm CreateMediaForm(AccountUser user)
        {
            var model = new AjaxForm()
            {
                PostbackUrl = "https://localhost:7220/api/mediablob/image",
                Type = PostbackType.POST,
                Event = "channel:create:modal",
                Label = "Create",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Label = "File",
                        Name = "File",
                        FieldType = FormFieldTypes.file,
                        //Accept = ".png|.jpg",
                        Placeholder = "File",
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
        #endregion
        #region Login
        public AccountFormPage GetLoginPage(string? username)
        {
            var viewModel = new AccountFormPage(_contentService.CreatePageBaseContent());
            viewModel.Title = "Login";
            viewModel.Form = this.CreateLoginForm(username);
            viewModel.Links = CreateLoginLinks();
            return viewModel;
        }
        public Form CreateLoginForm(string? username)
        {
            var model = new Form()
            {
                Label = "Login",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "Username",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Username",
                        AriaInvalid = false,
                        Value = username
                    },
                    new FormField()
                    {
                        Name = "Password",
                        Placeholder = "Password",
                        FieldType = FormFieldTypes.password,
                        AriaInvalid = false,
                    }
                }
            };
            return model;
        }
        private List<NavigationItem> CreateLoginLinks()
        {
            return new List<NavigationItem>()
            {
                new NavigationItem()
                {
                    Href = _contentService.GetActionUrl(nameof(AccountController.Register), ControllerHelper.NameOf<AccountController>()),
                    Text = nameof(AccountController.Register),
                    Class = "secondary"
                },
                new NavigationItem()
                {
                    Href = _contentService.GetActionUrl(nameof(AccountController.ForgotPassword), ControllerHelper.NameOf<AccountController>()),
                    Text = "Forgot Password",
                    Class = "contrast",
                },
            };
        }
        #endregion
        #region Register
        public AccountFormPage GetRegisterPage(string? username, string? email)
        {
            var viewModel = new AccountFormPage(_contentService.CreatePageBaseContent());
            viewModel.Title = "Register"; 
            viewModel.Form = this.CreateRegisterForm(username, email);
            viewModel.Links = CreateRegisterLinks();
            return viewModel;
        }
        private List<NavigationItem> CreateRegisterLinks()
        {
            return new List<NavigationItem>()
            {
                new NavigationItem()
                {
                    Href = _contentService.GetActionUrl(nameof(AccountController.Login), ControllerHelper.NameOf<AccountController>()),
                    Text = nameof(AccountController.Login),
                    Class = "secondary"
                },
                new NavigationItem()
                {
                    Href = _contentService.GetActionUrl(nameof(AccountController.ForgotPassword), ControllerHelper.NameOf<AccountController>()),
                    Text = "Forgot Password",
                    Class = "contrast",
                },
            };
        }
        public Form CreateRegisterForm(string? username, string? email)
        {
            var model = new Form()
            {
                Label = "Register",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "Username",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Username",
                        AriaInvalid = false,
                        Value = username
                    },
                    new FormField()
                    {
                        Name = "Email",
                        FieldType = FormFieldTypes.email,
                        Placeholder = "Email",
                        AriaInvalid = false,
                        Value = email
                    },
                    new FormField()
                    {
                        Name = "Password",
                        Placeholder = "Password",
                        FieldType = FormFieldTypes.password,
                        AriaInvalid = false,
                    },
                    new FormField()
                    {
                        Name = "ConfirmPassword",
                        Placeholder = "Confirm Password",
                        FieldType = FormFieldTypes.password,
                        AriaInvalid = false,
                    }
                }
            };
            return model;
        }
        #endregion
        #region ForgotPassword
        public AccountFormPage GetForgotPasswordConfirmationPage()
        {
            var viewModel = new AccountFormPage(_contentService.CreatePageBaseContent());
            viewModel.Title = "Forgot password";
            viewModel.Form = this.CreateForgotPasswordConfirmationForm();
            viewModel.Links = this.CreateForgotPasswordLinks();
            return viewModel;
        }
        public Form CreateForgotPasswordConfirmationForm()
        {
            var model = new Form()
            {
                Label = "Confirm Account",
            };
            return model;
        }
        public AccountFormPage GetForgotPasswordPage(string? usernameOrEmail)
        {
            var viewModel = new AccountFormPage(_contentService.CreatePageBaseContent());
            viewModel.Title = "Forgot password";
            viewModel.Form = this.CreateForgotPasswordForm(usernameOrEmail);
            viewModel.Links = this.CreateForgotPasswordLinks();
            return viewModel;
        }
        public Form CreateForgotPasswordForm(string? usernameOrEmail)
        {
            var model = new Form()
            {
                Label = "Forgot Password",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "Username",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Username or email",
                        AriaInvalid = false,
                        Hidden = false,
                        Value = usernameOrEmail
                    },
                }
            };
            return model;
        }
        private List<NavigationItem> CreateForgotPasswordLinks()
        {
            return new List<NavigationItem>()
            {
                new NavigationItem()
                {
                    Href = _contentService.GetActionUrl(nameof(AccountController.Login), ControllerHelper.NameOf<AccountController>()),
                    Text = nameof(AccountController.Login),
                    Class = "secondary"
                },
                new NavigationItem()
                {
                    Href = _contentService.GetActionUrl(nameof(AccountController.Register), ControllerHelper.NameOf<AccountController>()),
                    Text = nameof(AccountController.Register),
                    Class = "contrast",
                },
            };
        }
        #endregion
        #region ConfirmAccount
        public AccountFormPage GetConfirmAccountPage(string? userId)
        {
            var viewModel = new AccountFormPage(_contentService.CreatePageBaseContent());
            viewModel.Title = "Confirm your account";
            viewModel.Form = this.CreateConfirmAccountForm(userId);
            viewModel.Links = this.CreateResetPasswordLinks();
            return viewModel;
        }
        public Form CreateConfirmAccountForm(string? userId)
        {
            var model = new Form()
            {
                Label = "Confirm Account",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "Username",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Username or email",
                        AriaInvalid = false,
                        Value = userId
                    },
                }
            };
            return model;
        }
        #endregion
        #region ResetPassword 
        public AccountFormPage GetResetPasswordPage(string? userId, string? code)
        {
            var viewModel = new AccountFormPage(_contentService.CreatePageBaseContent());
            viewModel.Title = "Reset password";
            viewModel.Form = this.CreateResetPasswordResetForm(userId, code);
            viewModel.Links = this.CreateResetPasswordLinks();
            return viewModel;
        }
        public Form CreateResetPasswordResetForm(string? userId, string? code)
        {
            var model = new Form()
            {
                Label = "Forgot Password",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "UserId",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Username or email",
                        AriaInvalid = false,
                        Hidden = true,
                        Value = userId
                    },
                    new FormField()
                    {
                        Name = "Code",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Code",
                        AriaInvalid = false,
                        Hidden = true,
                        Value = code
                    },
                    new FormField()
                    {
                        Name = "Password",
                        Label = "New Password",
                        FieldType = FormFieldTypes.password,
                        Placeholder = "New Password",
                        AriaInvalid = false,
                        Value = null
                    },
                    new FormField()
                    {
                        Name = "ConfirmPassword",
                        FieldType = FormFieldTypes.password,
                        Placeholder = "Confirm Password",
                        AriaInvalid = false,
                        Value = null
                    },
                }
            };
            return model;
        }
        private List<NavigationItem> CreateResetPasswordLinks()
        {
            return new List<NavigationItem>()
            {
                new NavigationItem()
                {
                    Href = _contentService.GetActionUrl(nameof(AccountController.Login), ControllerHelper.NameOf<AccountController>()),
                    Text = nameof(AccountController.Login),
                    Class = "secondary"
                },
                new NavigationItem()
                {
                    Href = _contentService.GetActionUrl(nameof(AccountController.Register), ControllerHelper.NameOf<AccountController>()),
                    Text = nameof(AccountController.Register),
                    Class = "contrast",
                },
            };
        }
        #endregion
        public NavigationItem? GetPageLink(string route, string? name = null)
        {
            return new NavigationItem()
            {
                Text = name ?? route,
                Href = this._contentService.GetActionUrl(route, ControllerHelper.NameOf<AccountController>(), new { })
            };
        }
    }
}

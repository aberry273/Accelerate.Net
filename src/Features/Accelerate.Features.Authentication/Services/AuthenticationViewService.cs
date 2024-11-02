using Accelerate.Features.Authentication.Controllers;
using Accelerate.Features.Authentication.Models.Views;
using Accelerate.Foundations.Users.Models;
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Helpers;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.UI.Components.Table;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Media.Models.Data;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Aggregations;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Security.Policy;
using System.Threading.Channels;
using static System.Net.WebRequestMethods;

namespace Accelerate.Features.Authentication.Services
{
    public class AuthenticationViewService : IAuthenticationViewService
    {
        private readonly SignInManager<UsersUser> _signInManager;
        private OAuthConfiguration _OAuthConfig;
        private IMetaContentService _contentService;
        public AuthenticationViewService(
            SignInManager<UsersUser> signInManager,
            IMetaContentService contentService,
            IOptions<OAuthConfiguration> options)
        {
            _signInManager = signInManager;
            _contentService = contentService;
            _OAuthConfig = options.Value;
        } 
        #region Login
        public async Task<AuthenticationFormPage> GetLoginPage(string? username)
        {
            var viewModel = new AuthenticationFormPage(_contentService.CreatePageBaseContent());
            viewModel.Title = "Login";
            viewModel.Form = this.CreateLoginForm(username);
            viewModel.Links = CreateLoginLinks();
            viewModel.ExternalLoginAction = "ExternalLogin";
            viewModel.ExternalLoginPostbackUrl = "/Account/ExternalLogin";
            var providers = await _signInManager.GetExternalAuthenticationSchemesAsync();
            viewModel.Providers = this.GetProviderLinks(providers);
            return viewModel;
        }
        private List<ProviderLink> GetProviderLinks(IEnumerable<AuthenticationScheme> providers)
        {
            var result = new List<ProviderLink>();
            foreach (var provider in providers)
            {
                result.Add(new ProviderLink()
                {
                    Name = provider.Name,
                    Color = GetProviderColor(provider.Name)
                });
            }
            return result;
        }
        private string GetProviderColor(string provider)
        {
            switch (provider)
            {
                case "Facebook":
                    return "#4267B2";
                case "Google":
                    return "#4285F4";
                case "Microsoft":
                    return "#000";
                default:
                    return "#4267B2";
            }
        }
        public Form CreateLoginForm(string? username)
        {
            var model = new Form()
            {
                Label = "Login",
                Type = PostbackType.POST,
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
        private List<NavigationItem> CreateSocialLinks()
        {
            //<a class="btn btn-primary btn-block btn-responsive" style="background-color: #3578E5; border-color: #3578E5; color: #ffffff; font-size:1.5rem; padding: 20px;" href="https://www.facebook.com/dialog/oauth?client_id=@Html.DisplayFor(model => model.FacebookAppId)&redirect_uri=@Html.DisplayFor(model => model.FacebookRedirectUri)&&response_type=code&scope=email"><i class="fab fa-facebook"></i>&nbsp;&nbsp;&nbsp;Login with Facebook</a>

            return new List<NavigationItem>()
            {
                new NavigationItem()
                {
                    Href = $"https://www.facebook.com/dialog/oauth?client_id={_OAuthConfig.FacebookAppId}&redirect_uri={_OAuthConfig.FacebookRedirectUri}&response_type=code&scope=email",
                    Text = "Facebook",
                    Class = "secondary",
                    Icon = "facebook"
                },
                /*
                 * https://accounts.google.com/o/oauth2/v2/auth?
                 scope=https%3A//www.googleapis.com/auth/drive.metadata.readonly&
                 access_type=offline&
                 include_granted_scopes=true&
                 response_type=code&
                 state=state_parameter_passthrough_value&
                 redirect_uri=https%3A//oauth2.example.com/code&
                 client_id=client_id
                */
                new NavigationItem()
                {
                    Href = $"https://accounts.google.com/o/oauth2/v2/auth?scope=https%3A//www.googleapis.com/auth/drive.metadata.readonly&access_type=offline&include_granted_scopes=true&response_type=code&state=state_parameter_passthrough_value&redirect_uri=${_OAuthConfig.GoogleRedirectUri}code&client_id=${_OAuthConfig.GoogleAppId}",
                    Text = "Google",
                    Class = "secondary",
                    Icon = "Google"
                },
            };
        }
        private List<NavigationItem> CreateLoginLinks()
        {
            return new List<NavigationItem>()
            {
                new NavigationItem()
                {
                    Href = _contentService.GetActionUrl(nameof(AuthenticationController.Register), ControllerHelper.NameOf<AuthenticationController>()),
                    Text = nameof(AuthenticationController.Register),
                    Class = "secondary"
                },
                new NavigationItem()
                {
                    Href = _contentService.GetActionUrl(nameof(AuthenticationController.ForgotPassword), ControllerHelper.NameOf<AuthenticationController>()),
                    Text = "Forgot Password",
                    Class = "secondary",
                },
            };
        }
        #endregion
        #region ExternalLoginExistingUser
        public async Task<AuthenticationFormPage> GetExternalLoginExistingUser(string? username, string providerName)
        {
            var viewModel = new AuthenticationFormPage(_contentService.CreatePageBaseContent());
            viewModel.Title = "Link Account";
            viewModel.Form = this.CreatelLoginExistingUserForm(username, providerName);
            viewModel.Links = CreateLoginLinks();
            viewModel.ExternalLoginPostbackUrl = "/Account/ExternalLoginExistingUser";
            viewModel.ExternalLoginAction = "ExternalLoginExistingUser";

            return viewModel;
        }
        public Form CreatelLoginExistingUserForm(string? username, string providerName)
        {
            var model = new Form()
            {
                Label = "Link",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "Link",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Username",
                        AriaInvalid = false,
                        Disabled = true,
                        Value = $"Continue to link your {providerName} account"
                    },
                    new FormField()
                    {
                        Name = "Username",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Username",
                        AriaInvalid = false,
                        Hidden = false,
                        Disabled = true,
                        Value = username
                    }
                }
            };
            return model;
        }
        #endregion
        #region ExternalLoginNewUser
        public async Task<AuthenticationFormPage> GetExternalLoginNewUser(string? username)
        {
            var viewModel = new AuthenticationFormPage(_contentService.CreatePageBaseContent());
            viewModel.Title = "Complete Account";
            viewModel.Form = this.CreatelLoginNewUserForm(username);
            viewModel.Links = CreateLoginLinks();
            viewModel.ExternalLoginAction = "ExternalLogin";
            viewModel.ExternalLoginPostbackUrl = "/Account/ExternalLoginNewUser"; 

            return viewModel;
        }
        public Form CreatelLoginNewUserForm(string? username)
        {
            var model = new Form()
            {
                Title = "Create a username",
                Label = "Submit",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "Username",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Username",
                        Min = 3,
                        Max = 20,
                        AriaInvalid = false,
                        Value = username
                    },
                }
            };
            return model;
        }
        #endregion
        #region ExternalLoginDeactivatedUser
        public async Task<AuthenticationFormPage> GetExternalLoginDeactivatedUser(string? username)
        {
            var viewModel = new AuthenticationFormPage(_contentService.CreatePageBaseContent());
            viewModel.Title = "Reactivate Account";
            viewModel.Form = this.CreatelLoginDeactivatedUserForm(username);
            viewModel.Links = CreateLoginLinks();
            viewModel.ExternalLoginAction = "ExternalLogin";
            viewModel.ExternalLoginPostbackUrl = "/Account/ExternalLoginDeactivatedUser";

            return viewModel;
        }
        public Form CreatelLoginDeactivatedUserForm(string? username)
        {
            var model = new Form()
            {
                Title = "Activate your account",
                Label = "Continue",
                Type = PostbackType.POST,
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "Message",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Message",
                        AriaInvalid = false,
                        Disabled = true,
                        Value = "This account has been de-activated, continue to re-active this account."
                    },
                    new FormField()
                    {
                        Name = "Username",
                        FieldType = FormFieldTypes.input,
                        Placeholder = "Username",
                        AriaInvalid = false,
                        Value = username,
                        Hidden = false,
                        Disabled = true
                    },
                }
            };
            return model;
        }
        #endregion
        #region Register
        public AuthenticationFormPage GetRegisterPage(string? username, string? email)
        {
            var viewModel = new AuthenticationFormPage(_contentService.CreatePageBaseContent());
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
                    Href = _contentService.GetActionUrl(nameof(AuthenticationController.Login), ControllerHelper.NameOf<AuthenticationController>()),
                    Text = nameof(AuthenticationController.Login),
                    Class = "secondary"
                },
                new NavigationItem()
                {
                    Href = _contentService.GetActionUrl(nameof(AuthenticationController.ForgotPassword), ControllerHelper.NameOf<AuthenticationController>()),
                    Text = "Forgot Password",
                    Class = "contrast",
                },
            };
        }
        public Form CreateRegisterForm(string? username, string? email)
        {
            var model = new Form()
            {
                Title = "Register by email",
                Label = "Register",
                Type = PostbackType.POST,
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
        public AuthenticationFormPage GetForgotPasswordConfirmationPage()
        {
            var viewModel = new AuthenticationFormPage(_contentService.CreatePageBaseContent());
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
        public AuthenticationFormPage GetForgotPasswordPage(string? usernameOrEmail)
        {
            var viewModel = new AuthenticationFormPage(_contentService.CreatePageBaseContent());
            viewModel.Title = "Forgot password";
            viewModel.Form = this.CreateForgotPasswordForm(usernameOrEmail);
            viewModel.Links = this.CreateForgotPasswordLinks();
            return viewModel;
        }
        public Form CreateForgotPasswordForm(string? usernameOrEmail)
        {
            var model = new Form()
            {
                Title = "Reset your password",
                Label = "Forgot Password",
                Type = PostbackType.POST,
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
                    Href = _contentService.GetActionUrl(nameof(AuthenticationController.Login), ControllerHelper.NameOf<AuthenticationController>()),
                    Text = nameof(AuthenticationController.Login),
                    Class = "secondary"
                },
                new NavigationItem()
                {
                    Href = _contentService.GetActionUrl(nameof(AuthenticationController.Register), ControllerHelper.NameOf<AuthenticationController>()),
                    Text = nameof(AuthenticationController.Register),
                    Class = "contrast",
                },
            };
        }
        #endregion
        #region ConfirmAccount
        public AuthenticationFormPage GetConfirmAccountPage(string? userId)
        {
            var viewModel = new AuthenticationFormPage(_contentService.CreatePageBaseContent());
            viewModel.Title = "Confirm your account";
            viewModel.Form = this.CreateConfirmAccountForm(userId);
            viewModel.Links = this.CreateResetPasswordLinks();
            return viewModel;
        }
        public Form CreateConfirmAccountForm(string? userId)
        {
            var model = new Form()
            {
                Title = "Confirm your account",
                Label = "Confirm Account",
                Type = PostbackType.POST,
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
        public AuthenticationFormPage GetResetPasswordPage(string? userId, string? code)
        {
            var viewModel = new AuthenticationFormPage(_contentService.CreatePageBaseContent());
            viewModel.Title = "Reset password";
            viewModel.Form = this.CreateResetPasswordResetForm(userId, code);
            viewModel.Links = this.CreateResetPasswordLinks();
            return viewModel;
        }
        public Form CreateResetPasswordResetForm(string? userId, string? code)
        {
            var model = new Form()
            {
                Label = "Reset Password",
                Type = PostbackType.POST,
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
                    Href = _contentService.GetActionUrl(nameof(AuthenticationController.Login), ControllerHelper.NameOf<AuthenticationController>()),
                    Text = nameof(AuthenticationController.Login),
                    Class = "secondary"
                },
                new NavigationItem()
                {
                    Href = _contentService.GetActionUrl(nameof(AuthenticationController.Register), ControllerHelper.NameOf<AuthenticationController>()),
                    Text = nameof(AuthenticationController.Register),
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
                Href = this._contentService.GetActionUrl(route, ControllerHelper.NameOf<AuthenticationController>(), new { })
            };
        }
    }
}

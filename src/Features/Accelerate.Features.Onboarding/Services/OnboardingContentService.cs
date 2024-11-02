using Accelerate.Features.Onboarding.Models.Views;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Kyc.Models.Entities;
using Accelerate.Foundations.Users.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Accelerate.Features.Onboarding.Services
{
    public class OnboardingContentService : IOnboardingContentService
    {
        UserManager<UsersUser> _userManager;
        IMetaContentService _metaContentService;
        ISharedContentService _sharedContentService;
        IEntityService<UsersProfile> _profileService;
        public OnboardingContentService(
            IMetaContentService metaContentService,
            ISharedContentService sharedContentService,
            IEntityService<UsersProfile> profileService,
            UserManager<UsersUser> userManager
            )
        {
            _sharedContentService = sharedContentService;
            _metaContentService = metaContentService;
            _userManager = userManager;
            _profileService = profileService;
        }

        protected async Task<UsersUser> GetUserWithProfile(ClaimsPrincipal principle)
        {
            var user = await _userManager.GetUserAsync(principle);
            if (user == null)
            {
                return null;
            }
            var profile = _profileService.Get(user.UsersProfileId.GetValueOrDefault());
            user.UsersProfile = profile;
            return user;
        }

        public OnboardingBasePage CreateBasePage(UserProfile profile)
        {
            var basePage = _metaContentService.CreatePageBaseContent(profile);
            var model = new OnboardingBasePage(basePage);
            model.Steps = this.CreateOnboardingSteps();
            return model;
        }
        public NavigationGroup CreateOnboardingSteps()
        {
            var items = new List<NavigationItem>()
                {
                    new NavigationItem()
                    {
                        Text = "Details",
                        Href = "/Onboarding/SignUp"
                    },
                    new NavigationItem()
                    {
                        Text = "Authenticate",
                        Href = "/Onboarding/AuthenticateOtp"
                    },
                    new NavigationItem()
                    {
                        Text = "Finalize",
                        Href = "/Onboarding/Finalize"
                    },
                };
            return new NavigationGroup()
            {
                Items = items,
                Selected = items?.FirstOrDefault()?.Text,
            };
        }
        #region Sign Up
        public async Task<OnboardingBasePage> CreateSignUpPage(ClaimsPrincipal userClaim)
        {
            var user = await GetUserWithProfile(userClaim);
            var userProfile = Foundations.Users.Helpers.UsersHelpers.CreateUserProfile(user);

            var viewModel = CreateBasePage(userProfile);

            var identityModel = new KycCheckIdentityEntity();
            viewModel.Form = this.CreateSignUpForm(user, identityModel);

            return viewModel;
        }
        public async Task<OnboardingBasePage> CreateConsumerSignUpPage(ClaimsPrincipal userClaim)
        {
            var user = await GetUserWithProfile(userClaim);
            var userProfile = Foundations.Users.Helpers.UsersHelpers.CreateUserProfile(user);

            var viewModel = CreateBasePage(userProfile);

            var identityModel = new KycCheckIdentityEntity();
            viewModel.Form = this.CreateConsumerSignUpForm(user, identityModel);

            return viewModel;
        }
        public async Task<OnboardingBasePage> CreateBusinessSignUpPage(ClaimsPrincipal userClaim)
        {
            var user = await GetUserWithProfile(userClaim);
            var userProfile = Foundations.Users.Helpers.UsersHelpers.CreateUserProfile(user);

            var viewModel = CreateBasePage(userProfile);

            var identityModel = new KycCheckIdentityEntity();
            viewModel.Form = this.CreateBusinessSignUpForm(user, identityModel);
            return viewModel;
        }
        public async Task<AuthenticateOtpPage> CreateAuthenticateOtpPage(ClaimsPrincipal userClaim, string provider)
        {
            var user = await GetUserWithProfile(userClaim);
            var userProfile = Foundations.Users.Helpers.UsersHelpers.CreateUserProfile(user);

            var model = CreateBasePage(userProfile);
            var viewModel = new AuthenticateOtpPage(model);
            viewModel.Steps.Selected = "Authenticate";
            var identityModel = new KycCheckIdentityEntity();
            viewModel.Form = this.CreateAuthenticateOtpForm(user.Id, identityModel, provider);
            viewModel.ResendForm = this.CreateResendAuthenticateOtpForm(user.Id, identityModel, provider);
            return viewModel;
        }
        public async Task<AuthenticateOtpPage> CreateAuthenticateOtpPage(Guid userId, string provider)
        {
            var userProfile = Foundations.Users.Helpers.UsersHelpers.CreateUserProfile(null);

            var model = CreateBasePage(userProfile);
            var viewModel = new AuthenticateOtpPage(model);
            viewModel.Steps.Selected = "Authenticate";
            var identityModel = new KycCheckIdentityEntity();
            viewModel.Form = this.CreateAuthenticateOtpForm(userId, identityModel, provider);
            viewModel.ResendForm = this.CreateResendAuthenticateOtpForm(userId, identityModel, provider);
            return viewModel;
        }
        public Form CreateSignUpForm(UsersUser user, KycCheckIdentityEntity item)
        {
            var model = new Form()
            {
                Title = "Sign up",
                Label = "Submit",
                Type = PostbackType.POST,
                Fields = new List<FormField>()
                {
                    _metaContentService.FormField("Id", FormFieldComponents.aclFieldInput, null, null, item.Id, true, true),
                    _metaContentService.FormFieldItems("CustomerType", FormFieldComponents.aclFieldSelect, _sharedContentService.GetCustomerTypes(), null, null, _sharedContentService.GetCustomerTypes().FirstOrDefault(), false, false, null, null, null, "Is this for a business or individual?"),
                }
            };
            return model;
        }
        public Form CreateAuthenticateOtpForm(Guid userId, KycCheckIdentityEntity item, string provider)
        {
            var model = new Form()
            {
                Title = "Sign up",
                Label = "Submit",
                Type = PostbackType.POST,
                Fields = new List<FormField>()
                {
                    _metaContentService.FormField("Id", FormFieldComponents.aclFieldInput, null, null, item.Id, true, true),
                    _metaContentService.FormField("UserId", FormFieldComponents.aclFieldInput, null, null, userId, true, true),
                    _metaContentService.FormField("Code", FormFieldComponents.aclFieldInput, null, null, null, false, false, null, null, null, "Enter your one-time password"),
                    _metaContentService.FormField("Provider", FormFieldComponents.aclFieldInput, null, null, provider, false, hidden:true, null, null, null),
                }
            };
            return model;
        }
        public AjaxForm CreateResendAuthenticateOtpForm(Guid userId, KycCheckIdentityEntity item, string provider)
        {
            var model = new AjaxForm()
            {
                Text = "Resend authentication code",
                Label = "Resend",
                Action = $"/{Foundations.Common.Constants.ApiPaths.VersionPath}/onboarding/authentication/resendcode",
                Type = PostbackType.POST,
                Event = "post:created",
                ActionEvent = "action:post",
                Fields = new List<FormField>()
                {
                    _metaContentService.FormField("Id", FormFieldComponents.aclFieldInput, null, null, item.Id, true, true),
                    _metaContentService.FormField("UserId", FormFieldComponents.aclFieldInput, null, null, userId, true, true),
                    _metaContentService.FormField("Provider", FormFieldComponents.aclFieldInput, null, null, provider, false, hidden:true, null, null, null),
                }
            };
            return model;
        }
        public Form CreateConsumerSignUpForm(UsersUser user, KycCheckIdentityEntity item)
        {
            var model = new Form()
            {
                Title = "Consumer Sign up",
                Label = "Submit",
                Type = PostbackType.POST,
                Fields = new List<FormField>()
                {
                    _metaContentService.FormField("Id", FormFieldComponents.aclFieldInput, null, null, item.Id, true, true),
                    _metaContentService.FormField("Firstname", FormFieldComponents.aclFieldInput, null, null, null, false, false, null, null, null, "Firstname"),
                    _metaContentService.FormField("Lastname", FormFieldComponents.aclFieldInput, null, null, null, false, false, null, null, null, "Lastname"),
                    _metaContentService.FormField("Email", FormFieldComponents.aclFieldInput, null, null, null, false, false, null, null, null, "Email"),
                    _metaContentService.FormFieldItems("Country", FormFieldComponents.aclFieldSelect, _sharedContentService.GetCountryCodes(), null, null, null, false, false, null, null, null, "Country"),
                }
            };
            return model;
        }
        public Form CreateBusinessSignUpForm(UsersUser user, KycCheckIdentityEntity item)
        {
            var model = new Form()
            {
                Title = "Merchant Sign up",
                Label = "Submit",
                Type = PostbackType.POST,
                Fields = new List<FormField>()
                {
                    _metaContentService.FormField("Id", FormFieldComponents.aclFieldInput, null, null, item.Id, true, true),
                    _metaContentService.FormField("Firstname", FormFieldComponents.aclFieldInput, null, null, null, false, false, null, null, null, "Firstname"),
                    _metaContentService.FormField("Lastname", FormFieldComponents.aclFieldInput, null, null, null, false, false, null, null, null, "Lastname"),
                    _metaContentService.FormField("Email", FormFieldComponents.aclFieldInput, null, null, null, false, false, null, null, null, "Email"),
                    _metaContentService.FormField("CompanyName", FormFieldComponents.aclFieldInput, null, null, null, false, false, null, null, null, "Company Name"),
                    _metaContentService.FormField("Website", FormFieldComponents.aclFieldInput, null, null, null, false, false, null, null, null, "Company Website"),
                    _metaContentService.FormField("Volume", FormFieldComponents.aclFieldInput, null, null, null, false, false, null, null, null, "Estimated monthly volume (INR)"),
                    _metaContentService.FormFieldItems("Industry", FormFieldComponents.aclFieldSelect, _sharedContentService.GetIndustries(), null, null, null, false, false, null, null, null, "Industry"),
                    _metaContentService.FormFieldItems("Country", FormFieldComponents.aclFieldSelect, _sharedContentService.GetCountryCodes(), null, null, null, false, false, null, null, null, "Country"),
                }
            };
            return model;
        }
        #endregion
        #region Identity Check
        public async Task<OnboardingBasePage> CreateIdentityCheckPage(ClaimsPrincipal userClaim)
        {
            var user = await GetUserWithProfile(userClaim);
            var userProfile = Foundations.Users.Helpers.UsersHelpers.CreateUserProfile(user);

            var viewModel = CreateBasePage(userProfile);

            var identityModel = new KycCheckIdentityEntity();
            viewModel.Form = this.CreateIdentityCheckForm(user.Id, identityModel);

            return viewModel;
        }
        public async Task<OnboardingBasePage> CreateIdentityCheckPage(Guid userId)
        {
            var user = await GetUserWithProfile(null);
            var userProfile = Foundations.Users.Helpers.UsersHelpers.CreateUserProfile(user);

            var viewModel = CreateBasePage(userProfile);

            var identityModel = new KycCheckIdentityEntity();
            viewModel.Form = this.CreateIdentityCheckForm(userId, identityModel);

            return viewModel;
        }
        public AjaxForm CreateIdentityCheckForm(Guid userId, KycCheckIdentityEntity item)
        {
            var model = new AjaxForm()
            {
                Action = $"/api/onboarding/identity/{item.Id}",
                Type = PostbackType.DELETE,
                Event = $"onboarding:identity:modal",
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
                        Value = userId,
                    }
                }
            };
            return model;
        }
        #endregion
    }
}

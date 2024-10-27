using Accelerate.Features.Onboarding.Models.Views;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.UI.Components;
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
        IEntityService<UsersProfile> _profileService;
        public OnboardingContentService(
            IMetaContentService metaContentService,
            IEntityService<UsersProfile> profileService,
            UserManager<UsersUser> userManager
            )
        {
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

            return model;
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
        private static List<dynamic> GetIndustries()
        {
            return new List<dynamic>(){
                "Fintech"
            };
        }
        private static List<dynamic> GetCountryCodes()
        {
            return new List<dynamic>(){
                "IND"
            };
        }
        public Form CreateSignUpForm(UsersUser user, KycCheckIdentityEntity item)
        {
            var model = new Form()
            {
                Title = "Sign up",
                Label = "Submit",
                Fields = new List<FormField>()
                {
                    _metaContentService.FormField("Id", FormFieldComponents.aclFieldInput, null, null, item.Id, true, true),
                    _metaContentService.FormField("Firstname", FormFieldComponents.aclFieldInput, null, null, null, false, false, null, null, null, "Firstname"),
                    _metaContentService.FormField("Lastname", FormFieldComponents.aclFieldInput, null, null, null, false, false, null, null, null, "Lastname"),
                    _metaContentService.FormField("Email", FormFieldComponents.aclFieldInput, null, null, null, false, false, null, null, null, "Email"),
                    _metaContentService.FormField("CompanyName", FormFieldComponents.aclFieldInput, null, null, null, false, false, null, null, null, "Company Name"),
                    _metaContentService.FormField("Website", FormFieldComponents.aclFieldInput, null, null, null, false, false, null, null, null, "Company Website"),
                    _metaContentService.FormFieldItems("Industry", FormFieldComponents.aclFieldSelect, GetIndustries(), null, null, null, false, false, null, null, null, "Industry"),
                    _metaContentService.FormFieldItems("What are you wanting to use Superstable", FormFieldComponents.aclFieldSelect, GetCountryCodes(), null, null, null, false, false, null, null, null, "Country"),
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
            viewModel.Form = this.CreateIdentityCheckForm(user, identityModel);

            return viewModel;
        }
        public AjaxForm CreateIdentityCheckForm(UsersUser user, KycCheckIdentityEntity item)
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
                        Value = user.Id,
                    }
                }
            };
            return model;
        }
        #endregion
    }
}

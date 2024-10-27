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

    }
}

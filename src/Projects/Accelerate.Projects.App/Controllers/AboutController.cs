using Accelerate.Foundations.Account.Attributes;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Features.Content.Controllers;
using System.Security.Claims;
using Accelerate.Foundations.Database.Services;
namespace Accelerate.Projects.App.Controllers
{
    //[Authorize]
    public class AboutController : BaseController
    {
        UserManager<AccountUser> _userManager;
        IMetaContentService _contentService;
        IEntityService<AccountProfile> _profileService;
        public AboutController(
            UserManager<AccountUser> userManager,
            IMetaContentService contentService,
            IEntityService<AccountProfile> profileService)
            : base(contentService)
        {
            _contentService = contentService;
            _userManager = userManager;
            _profileService = profileService;
        }
        private BasePage CreateBaseContent(AccountUser user)
        { 
            var profile = Accelerate.Foundations.Account.Helpers.AccountHelpers.CreateUserProfile(user);
            return _contentService.CreatePageBaseContent(profile);
        }

        private async Task<AccountUser> GetUserWithProfile(ClaimsPrincipal principle)
        {
            var user = await _userManager.GetUserAsync(principle);
            if (user == null)
            {
                return null;
            }
            var profile = _profileService.Get(user.AccountProfileId.GetValueOrDefault());
            user.AccountProfile = profile;
            return user;
        }
        public async Task<IActionResult> Index()
        {
            var user = await GetUserWithProfile(this.User);
          
            var viewModel = CreateBaseContent(user);

            return View(viewModel);
        }
        [HttpGet("About/Terms-And-Conditions")]
        public async Task<IActionResult> TermsAndConditions()
        {
            var user = await GetUserWithProfile(this.User);

            var viewModel = CreateBaseContent(user);

            return View(viewModel);
        }
        [HttpGet("About/Privacy-Policy")]
        public async Task<IActionResult> PrivacyPolicy()
        {
            var user = await GetUserWithProfile(this.User);

            var viewModel = CreateBaseContent(user);

            return View(viewModel);
        }
        [HttpGet("About/data-deletion")]
        public async Task<IActionResult> DataDeletion()
        {
            var user = await GetUserWithProfile(this.User);

            var viewModel = CreateBaseContent(user);

            return View(viewModel);
        }
    }
}

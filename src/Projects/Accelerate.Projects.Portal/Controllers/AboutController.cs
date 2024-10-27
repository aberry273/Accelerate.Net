using Accelerate.Foundations.Users.Attributes;
using Accelerate.Foundations.Users.Models.Entities;
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
        UserManager<UsersUser> _userManager;
        IMetaContentService _contentService;
        IEntityService<UsersProfile> _profileService;
        public AboutController(
            UserManager<UsersUser> userManager,
            IMetaContentService contentService,
            IEntityService<UsersProfile> profileService)
            : base(contentService)
        {
            _contentService = contentService;
            _userManager = userManager;
            _profileService = profileService;
        }
        private BasePage CreateBaseContent(UsersUser user)
        { 
            var profile = Accelerate.Foundations.Users.Helpers.UsersHelpers.CreateUserProfile(user);
            return _contentService.CreatePageBaseContent(profile);
        }

        private async Task<UsersUser> GetUserWithProfile(ClaimsPrincipal principle)
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

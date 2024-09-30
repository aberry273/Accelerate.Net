
using Accelerate.Features.Admin.Services;
using Accelerate.Foundations.Account.Attributes;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading;

namespace Accelerate.Features.Admin.Controllers
{ 
    public class AdminOldController: BaseController
    {
        protected SignInManager<AccountUser> _signInManager;
        UserManager<AccountUser> _userManager;
        IEntityService<AccountProfile> _profileService;
        IMetaContentService _contentService;
        IAdminViewService _adminViewService;
        const string _unauthenticatedRedirectUrl = "/Account/login";
        protected string _razorPath;
        private const string _notFoundRazorFile = "~/Views/Shared/NotFound.cshtml";
        public AdminOldController(
            SignInManager<AccountUser> signInManager,
            UserManager<AccountUser> userManager,
            IAdminViewService adminViewService,
            IEntityService<AccountProfile> profileService,
            IMetaContentService contentService) : base(contentService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _profileService = profileService;
            _contentService = contentService;
            _adminViewService = adminViewService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(this.User);
            return (user != null)
                ? RedirectToAction("Jobs")
                : RedirectToAction("Login");
        }

        protected async Task<AccountUser> GetUserWithProfile(ClaimsPrincipal principle)
        {
            var user = await _userManager.GetUserAsync(principle);
            if (user == null)
            {
                return null;
            }
            var profile = _profileService.Get(user.AccountProfileId);
            user.AccountProfile = profile;
            return user;
        }
        [HttpGet]
        [RedirectUnauthenticatedRoute(url = _unauthenticatedRedirectUrl)]
        public async Task<IActionResult> Jobs()
        {
            var user = await GetUserWithProfile(this.User);
            if (user == null) return RedirectToAction("Index", "Account");

            var viewModel = _adminViewService.CreateJobsPage(user);

            return View("/Views/Jobs/Index.cshtml", viewModel);
        }
    }
}
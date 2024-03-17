using Accelerate.Features.Account.Attributes;
using Accelerate.Features.Account.Models.Entities;
using Accelerate.Features.Account.Models.Views;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Features.Account.Controllers
{
    //[Authorize]
    public class AccountController : BaseController
    {
        private SignInManager<AccountUser> _signInManager;
        private UserManager<AccountUser> _userManager;
        private ISharedContentService _contentService;
        public AccountController(
            ISharedContentService contentService,
            SignInManager<AccountUser> signInManager,
            UserManager<AccountUser> userManager)
            : base(contentService)
        {
            _contentService = contentService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        private const string _profileUrl = "/Profile/Index";

        public new IActionResult Index()
        {
            return RedirectToAction("Login");
            var model = new AccountPage(_contentService.CreatePageBaseContent());
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        [RedirectAuthenticatedRoute(url = _profileUrl)]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            //await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            //ViewData["ReturnUrl"] = returnUrl;
            //var viewModel = await this.GetLoginViewModel();
            var viewModel = new LoginPage(_contentService.CreatePageBaseContent());
            return View(viewModel);
        }
    }
}

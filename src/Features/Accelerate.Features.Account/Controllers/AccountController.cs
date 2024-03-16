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
        public AccountController(
            SharedContentService contentService,
            SignInManager<AccountUser> signInManager,
            UserManager<AccountUser> userManager)
            : base(contentService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        private const string _profileUrl = "/Profile/Index";

        // 
        // GET: /HelloWorld/
        public string Index()
        {
            return "This is my default action...";
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
            var viewModel = new LoginPage();
            return View(viewModel);
        }
    }
}

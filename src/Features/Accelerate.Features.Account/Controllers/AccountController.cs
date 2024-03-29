using Accelerate.Foundations.Account.Attributes;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Features.Account.Models.Views;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Integrations.Elastic.Services;
using MassTransit.DependencyInjection;
using MassTransit;
using Accelerate.Features.Content.EventBus;
using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Accelerate.Features.Account.Models;
using Accelerate.Foundations.Account.Models;

namespace Accelerate.Features.Account.Controllers
{
    //[Authorize]
    public class AccountController : BaseController
    {
        readonly Bind<IAccountBus, IPublishEndpoint> _publishEndpoint;
        IElasticService<AccountUserDocument> _searchService;
        private SignInManager<AccountUser> _signInManager;
        private UserManager<AccountUser> _userManager;
        private IEmailSender<AccountUser> _emailSender;
        private IMetaContentService _contentService;
        private IEntityService<AccountProfile> _profileService;
        public AccountController(
            IMetaContentService contentService,
            SignInManager<AccountUser> signInManager,
            UserManager<AccountUser> userManager,
            IEmailSender<AccountUser> emailSender,
            IEntityService<AccountProfile> profileService,
            Bind<IAccountBus, IPublishEndpoint> publishEndpoint,
            IElasticService<AccountUserDocument> searchService)
            : base(contentService)
        {
            _contentService = contentService;
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _profileService = profileService;
            _publishEndpoint = publishEndpoint;
            _searchService = searchService;
        }

        private const string _authenticatedRedirectUrl = "/";
        private const string _unauthenticatedRedirectUrl = "/Account/login";


        private BasePage CreateBaseContent(AccountUser user)
        {
            var profile = user != null ? new UserProfile()
            {
                Username = user.UserName,
            } : null;
            return _contentService.CreatePageBaseContent(profile);
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(this.User);
            return  (user != null)
                ? RedirectToAction("Profile")
                : RedirectToAction("Login");
        }

        private bool IsEmail(string str)
        {
            return Regex.IsMatch(str,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        #region Pipelines

        private string GetTarget(AccountUser obj) => obj.Id.ToString();
        //private string GetTarget(AccountUser obj) => obj.TargetThread ?? obj.TargetChannel;
        protected async Task PostCreateSteps(AccountUser obj)
        {
            await _publishEndpoint.Value.Publish(new CreateDataContract<AccountUser>()
            {
                Data = obj,
                Target = GetTarget(obj),
                UserId = obj.Id
            });
        }
        protected async Task PostUpdateSteps(AccountUser obj)
        {
            await _publishEndpoint.Value.Publish(new UpdateDataContract<AccountUser>()
            {
                Data = obj,
                Target = GetTarget(obj),
                UserId = obj.Id
            });
        }
        protected async Task PostDeleteSteps(AccountUser obj)
        {
            await _publishEndpoint.Value.Publish(new DeleteDataContract<AccountUser>()
            {
                Data = obj,
                Target = GetTarget(obj),
                UserId = obj.Id
            });
        }
        #endregion

        #region Manage
        [HttpGet]
        [AllowAnonymous]
        [RedirectUnauthenticatedRoute(url = _unauthenticatedRedirectUrl)]
        public async Task<IActionResult> Manage(string returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(this.User);
            var pageModel = CreateBaseContent(user);
            var viewModel = new ManagePage(pageModel);
            return View(viewModel);
        }
        #endregion

        #region Login
        [HttpGet]
        [AllowAnonymous]
        [RedirectAuthenticatedRoute(url = _authenticatedRedirectUrl)]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            //ViewData["ReturnUrl"] = returnUrl;
            //var viewModel = await this.GetLoginViewModel();
            var viewModel = new LoginPage(_contentService.CreatePageBaseContent());
            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [RedirectAuthenticatedRoute(url = _authenticatedRedirectUrl)]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginForm request, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var viewModel = new LoginPage(_contentService.CreatePageBaseContent());
            if (ModelState.IsValid)
            {
                // This does not count login failures towards account lockout
                // To enable password failures to trigger account lockout,
                // set lockoutOnFailure: true
                var user = IsEmail(request.Username) ? await _userManager.FindByEmailAsync(request.Username) : await _userManager.FindByNameAsync(request.Username);

                var result = user != null
                    ? await _signInManager.PasswordSignInAsync(user.UserName, request.Password, request.RememberMe.GetValueOrDefault(), lockoutOnFailure: false)
                    : null;
                if (result == null)
                {
                    viewModel.Form.Response = "No account with that email/username could be found";
                    return View(viewModel);
                }
                if (result.Succeeded)
                {
                    return RedirectToLocal(returnUrl ?? _authenticatedRedirectUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(LoginWith2FA), new { returnUrl, request.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    viewModel.Form.Response = "User account locked out";
                    viewModel.Form.Username = request.Username;
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    viewModel.Form.Response = "Invalid login attempt";
                    viewModel.Form.Username = request.Username;
                    return View(viewModel);
                }
            }
            // If execution got this far, something failed, redisplay the form.
            return View(viewModel);
        }


        [HttpGet]
        [AllowAnonymous]
        [RedirectAuthenticatedRoute(url = _authenticatedRedirectUrl)]
        public async Task<IActionResult> LoginWith2FA(bool rememberMe, string returnUrl = null)
        {
            // Ensure that the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var viewModel = new LoginPage(_contentService.CreatePageBaseContent());
            ViewData["ReturnUrl"] = returnUrl;

            return View(viewModel);
        }
        #endregion

        #region Register
        [HttpGet]
        [AllowAnonymous]
        [RedirectAuthenticatedRoute(url = _authenticatedRedirectUrl)]
        public async Task<IActionResult> Register(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            //ViewData["ReturnUrl"] = returnUrl;
            //var viewModel = await this.GetLoginViewModel();
            var viewModel = new RegisterPage(_contentService.CreatePageBaseContent());
            return View(viewModel);
        }
        [HttpPost]
        [AllowAnonymous]
        [RedirectAuthenticatedRoute(url = _authenticatedRedirectUrl)]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterForm request, string returnUrl = null)
        {
            var viewModel = new RegisterPage(_contentService.CreatePageBaseContent());
            try
            {
                ViewData["ReturnUrl"] = returnUrl;
                if (ModelState.IsValid)
                {
                    var user = new AccountUser { UserName = request.Username, Email = request.Email, Domain = "Public" };
                    var result = await _userManager.CreateAsync(user, request.Password);
                    if (result.Succeeded)
                    {
                        // Create profile
                        await _profileService.Create(new AccountProfile() { UserId = user.Id });
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                        //TODO: Move this to the pipeline
                        var callbackUrl = EmailConfirmationLink(user.UserName, code, Request.Scheme);
                        await _emailSender.SendConfirmationLinkAsync(user, user.Email, callbackUrl);

                        await _signInManager.PasswordSignInAsync(user, request.Password, isPersistent: false, lockoutOnFailure: false);
                        //Run pipeline
                        await PostCreateSteps(user);
                        StaticLoggingService.Log("User created a new account with password.");

                        return RedirectToLocal(returnUrl ?? _authenticatedRedirectUrl);
                    }
                    viewModel.Form.Response = result.Errors.FirstOrDefault().Description;
                    StaticLoggingService.LogError(result.Errors.FirstOrDefault().Description);
                }
                else
                {
                    var errors = ModelState.Select(x => x.Value?.Errors?.FirstOrDefault()?.ErrorMessage);
                    viewModel.Form.Response = string.Join(",", errors);
                }
                /*
                viewModel.LoginLink = new SimpleLinkModel()
                {
                    Href = LoginLink(),
                    IsInternal = true,
                    Text = "Login",
                    NewWindow = false
                };
                */
                // If execution got this far, something failed, redisplay the form.
                return View(viewModel);
            }
            catch (Exception ex)
            {
                /*
                viewModel.LoginLink = new SimpleLinkModel()
                {
                    Href = LoginLink(),
                    IsInternal = true,
                    Text = "Login",
                    NewWindow = false
                };
                */
                viewModel.Form.Response = "There was an error processing your request";
                return View(viewModel);
            }

        }
        #endregion

        #region Account Confirmation

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmAccount(string userId = null, string code = null)
        {
            var viewModel = new ConfirmPage(_contentService.CreatePageBaseContent());
            viewModel.Form.UserId = userId;
            viewModel.Form.Code = code;
            if (userId == null)
            {
                viewModel.Form.Response = "There was an error with the link that was supplied.";
                return View(viewModel);
            }
            if (code == null)
            {
                viewModel.Form.Response = "A code must be supplied for password reset.";
                return View(viewModel);
            }
            return View(viewModel);
        }
        #endregion

        public string EmailConfirmationLink(string userId, string code, string scheme)
        {
            return this.Url.Action(
                action: nameof(ConfirmAccount),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            var viewModel = new LoginPage(_contentService.CreatePageBaseContent());
            return View(viewModel);
        }
    }
}

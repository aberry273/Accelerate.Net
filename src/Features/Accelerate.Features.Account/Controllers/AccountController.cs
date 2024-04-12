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
using Accelerate.Features.Account.Services;
using Elastic.Clients.Elasticsearch;
using Accelerate.Features.Account.Models.Data;
using System.Security.Claims;
using Accelerate.Foundations.Common.Helpers;

namespace Accelerate.Features.Account.Controllers
{
    //[Authorize]
    public class AccountController : BaseController
    {
        readonly Bind<IAccountBus, IPublishEndpoint> _publishEndpoint;
        IElasticService<AccountUserDocument> _searchService;
        private SignInManager<AccountUser> _signInManager;
        private UserManager<AccountUser> _userManager;
        private IAccountViewService _accountViewService;
        private IEmailSender<AccountUser> _emailSender;
        private IMetaContentService _contentService;
        private IEntityService<AccountProfile> _profileService;
        public AccountController(
            IMetaContentService contentService,
            SignInManager<AccountUser> signInManager,
            UserManager<AccountUser> userManager,
            IEmailSender<AccountUser> emailSender,
            IAccountViewService accountViewService,
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
            _accountViewService = accountViewService;
        }

        private const string _authenticatedRedirectUrl = "/";
        private const string _unauthenticatedRedirectUrl = "/Account/login";
        private const string _accountFormRazorFile = "~/Views/Account/AccountFormPage.cshtml";
        
        private async Task<AccountUser> GetUserWithProfile(ClaimsPrincipal principle)
        {
            var user = await _userManager.GetUserAsync(principle);
            if (user == null) return null;
            var profile = _profileService.Get(user.AccountProfileId);
            user.AccountProfile = profile;
            return user;
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
        #region Manage
        [HttpGet]
        [AllowAnonymous]
        [RedirectUnauthenticatedRoute(url = _unauthenticatedRedirectUrl)]
        public async Task<IActionResult> Manage(string returnUrl = null)
        {
            var user = await GetUserWithProfile(this.User);
            var viewModel = _accountViewService.GetManagePage(user);
            return View(viewModel);
        }
        #endregion

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
         
        #region ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        [RedirectAuthenticatedRoute(url = _authenticatedRedirectUrl)]
        public async Task<IActionResult> ForgotPassword(string? username, string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            //ViewData["ReturnUrl"] = returnUrl;
            //var viewModel = await this.GetLoginViewModel();
            var viewModel = _accountViewService.GetForgotPasswordPage(username);
           
            return View(_accountFormRazorFile, viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordForm model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username) ?? await _userManager.FindByEmailAsync(model.Username);
                
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ConfirmAccount));
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = ResetPasswordCallbackLink(user.Id.ToString(), code, Request.Scheme);
                await _emailSender.SendConfirmationLinkAsync(user, "Reset Password",
                   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // If execution got this far, something failed, redisplay the form.
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        [RedirectAuthenticatedRoute(url = _authenticatedRedirectUrl)]
        public async Task<IActionResult> ForgotPasswordConfirmation(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            //ViewData["ReturnUrl"] = returnUrl;
            //var viewModel = await this.GetLoginViewModel();
            var viewModel = _accountViewService.GetForgotPasswordConfirmationPage();
            viewModel.Form.Response = "Please check your email to reset your password";
            return View(_accountFormRazorFile, viewModel);
        }
        #endregion

        #region ResetPassword

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string userId = null, string code = null)
        {
            var viewModel = _accountViewService.GetResetPasswordPage(userId, code);

            if (userId == null)
            {
                viewModel.Form.Response = "There was an error with the link that was supplied. ";
            }

            if (code == null)
            {
                viewModel.Form.Response += "A code must be supplied for password reset.";
            }

            return View(_accountFormRazorFile, viewModel);
        }
        public string ResetPasswordCallbackLink(string userId, string code, string scheme)
        {
            return this.Url.Action(
                action: nameof(ResetPassword),
                controller: ControllerHelper.NameOf<AccountController>(),
                values: new { userId, code },
                protocol: scheme);
        }
        #endregion
        #region Login
        [HttpGet]
        [AllowAnonymous]
        [RedirectAuthenticatedRoute(url = _authenticatedRedirectUrl)]
        public async Task<IActionResult> Login(string? username = null, string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewData["ReturnUrl"] = returnUrl; 
            var viewModel = _accountViewService.GetLoginPage(username);
             
            return View(_accountFormRazorFile, viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [RedirectAuthenticatedRoute(url = _authenticatedRedirectUrl)]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginForm request, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var viewModel = new AccountFormPage(_contentService.CreatePageBaseContent());
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
                    return View(_accountFormRazorFile, viewModel);
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
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    viewModel.Form.Response = "Invalid login attempt";
                    return View(_accountFormRazorFile, viewModel);
                }
            }
            // If execution got this far, something failed, redisplay the form.
            return View(_accountFormRazorFile, viewModel);
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

            var viewModel = new AccountFormPage(_contentService.CreatePageBaseContent());
            ViewData["ReturnUrl"] = returnUrl;

            return View(viewModel);
        }
        #endregion

        #region Register
        [HttpGet]
        [AllowAnonymous]
        [RedirectAuthenticatedRoute(url = _authenticatedRedirectUrl)]
        public async Task<IActionResult> Register(string? username, string? email, string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            //ViewData["ReturnUrl"] = returnUrl;
            //var viewModel = await this.GetLoginViewModel();
            var viewModel = _accountViewService.GetRegisterPage(username, email);

            return View(_accountFormRazorFile, viewModel);
        }
        [HttpPost]
        [AllowAnonymous]
        [RedirectAuthenticatedRoute(url = _authenticatedRedirectUrl)]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterForm request, string returnUrl = null)
        {
            var viewModel = _accountViewService.GetRegisterPage(request.Username, request.Email);
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
                        var profileId = await _profileService.CreateWithGuid(new AccountProfile() { UserId = user.Id });
                        // Get user and update with profile id
                        var accountUser = await _userManager.FindByNameAsync(request.Username);
                        accountUser.AccountProfileId = profileId.GetValueOrDefault();
                        await _userManager.UpdateAsync(accountUser);

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
                return View(_accountFormRazorFile, viewModel);
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
                return View(_accountFormRazorFile, viewModel);
            }

        }
        #endregion

        #region Account Confirmation

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmAccount(string username = null, string? response = null)
        {
            var viewModel = _accountViewService.GetConfirmAccountPage(username);
            viewModel.Form.Response = response;
            return View(_accountFormRazorFile, viewModel);
        }
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmAccount(ForgotPasswordForm model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username) ?? await _userManager.FindByEmailAsync(model.Username);

                if (user == null)
                {
                    var viewModel = _accountViewService.GetConfirmAccountPage(model.Username);
                    var response = "No user matching this username or email can be found";
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ConfirmAccount), new { model.Username, response });
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = ResetPasswordCallbackLink(user.Id.ToString(), code, Request.Scheme);
                await _emailSender.SendConfirmationLinkAsync(user, "Confirm Account",
                   $"Please confirm your account by clicking here: <a href='{callbackUrl}'>link</a>");
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // If execution got this far, something failed, redisplay the form.
            return View(model);
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
            var viewModel = new AccountFormPage(_contentService.CreatePageBaseContent());
            return View(viewModel);
        }
    }
}

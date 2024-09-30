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
using System.Text.Encodings.Web;
using System.Web;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Media.Models.Data;
using Accelerate.Foundations.Content.Services;
using Accelerate.Foundations.Common.Extensions;
using Twilio.TwiML.Messaging;
using Microsoft.AspNetCore.Authentication.Google;
using Accelerate.Foundations.Content.Models.Entities;
using static MassTransit.ValidationResultExtensions;

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
        IContentPostElasticService _contentElasticSearchService;
        private IEmailSender<AccountUser> _emailSender;
        private IMetaContentService _contentService;
        IContentActivityElasticService _contentActivityElasticService;
        IEntityService<ContentPostActivityEntity> _postActivityEntityService;
        IElasticService<ContentPostDocument> _postSearchService;
        IElasticService<MediaBlobDocument> _mediaSearchService;
        private IEntityService<AccountProfile> _profileService;
        public AccountController(
            IMetaContentService contentService,
            SignInManager<AccountUser> signInManager,
            UserManager<AccountUser> userManager,
            IEmailSender<AccountUser> emailSender,
            IAccountViewService accountViewService,
            IEntityService<AccountProfile> profileService,
            IContentPostElasticService contentElasticSearchService,
            IContentActivityElasticService contentActivityElasticService,
            IEntityService<ContentPostActivityEntity> postActivityEntityService,
            IElasticService<ContentPostDocument> postSearchService,
            IElasticService<MediaBlobDocument> mediaSearchService,
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
            _contentElasticSearchService = contentElasticSearchService;
            _contentActivityElasticService = contentActivityElasticService;
            _postActivityEntityService = postActivityEntityService;
            _postSearchService = postSearchService;
            _mediaSearchService = mediaSearchService;
            _accountViewService = accountViewService;
            // Move to service
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
        /*
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
        */
        #region Profile
        [HttpGet]
        [RedirectUnauthenticatedRoute(url = _unauthenticatedRedirectUrl)]
        public async Task<IActionResult> Profile(string returnUrl = null)
        {
            var user = await GetUserWithProfile(this.User);
            if (user == null) return RedirectToAction(nameof(Login));

            var viewModel = _accountViewService.GetManagePage(user);
            return View(viewModel);
        }
        #endregion
        #region Posts
        [HttpGet]
        [RedirectUnauthenticatedRoute(url = _unauthenticatedRedirectUrl)]
        public async Task<IActionResult> Posts(string returnUrl = null)
        {
            var user = await GetUserWithProfile(this.User);
            if (user == null)  return RedirectToAction(nameof(Login));

            var viewModel = _accountViewService.GetManagePage(user);
            viewModel.ActionUrl = "/api/contentpostactivity";
            viewModel.SearchUrl = $"/api/contentsearch/posts/{user.Id}";
            var aggResponse = await _postSearchService.GetAggregates(_contentElasticSearchService.CreateUserPostQuery(user.Id));
            viewModel.Filters = _accountViewService.CreatePostNavigationFilters(aggResponse);
            return View(viewModel);
        }
        #endregion
        #region Notifications
        [HttpGet]
        [RedirectUnauthenticatedRoute(url = _unauthenticatedRedirectUrl)]
        public async Task<IActionResult> Notifications(string returnUrl = null)
        {
            var user = await GetUserWithProfile(this.User);
            if (user == null) return RedirectToAction(nameof(Login));

            var totalActivities = _postActivityEntityService.Count(x => x.UserId == user.Id);
            var activities = _postActivityEntityService.Find(x => x.UserId == user.Id).ToList();

            var viewModel = _accountViewService.GetNotificationsPage(user, activities, totalActivities);
            viewModel.ActionUrl = "/api/contentpostactivity";
            viewModel.SearchUrl = $"/api/contentsearch/posts/{user.Id}";
            var aggResponse = await _postSearchService.GetAggregates(_contentElasticSearchService.CreateUserPostQuery(user.Id));
            viewModel.Filters = _accountViewService.CreatePostNavigationFilters(aggResponse);
            return View(viewModel);
        }
        #endregion
        #region Mentions
        [HttpGet]
        [RedirectUnauthenticatedRoute(url = _unauthenticatedRedirectUrl)]
        public async Task<IActionResult> Mentions(string returnUrl = null)
        {
            var user = await GetUserWithProfile(this.User);
            if (user == null) return RedirectToAction(nameof(Login));

            var viewModel = _accountViewService.GetMentionsPage(user);
            viewModel.ActionUrl = "/api/contentpostmentions";
            viewModel.SearchUrl = $"/api/contentsearch/mentions/{user.Id}";
            var aggResponse = await _postSearchService.GetAggregates(_contentElasticSearchService.CreateUserPostQuery(user.Id));
            viewModel.Filters = _accountViewService.CreatePostNavigationFilters(aggResponse);
            return View(viewModel);
        }
        #endregion
        #region Media
        public RequestQuery<MediaBlobDocument> CreateMediasAggregateQuery()
        {
            var filters = new List<QueryFilter>()
            {
                //this.Filter(Foundations.Content.Constants.Fields.channelId, ElasticCondition.Filter, channelId)
            };
            var aggregates = new List<string>()
            {
                Foundations.Media.Constants.Fields.Type.ToCamelCase(),
                Foundations.Media.Constants.Fields.Tags.ToCamelCase(),
            };
            return new RequestQuery<MediaBlobDocument>() { Filters = filters, Aggregates = aggregates };
        }
        [HttpGet]
        [RedirectUnauthenticatedRoute(url = _unauthenticatedRedirectUrl)]
        public async Task<IActionResult> Media(string returnUrl = null)
        {
            var user = await GetUserWithProfile(this.User);
            if (user == null) return RedirectToAction(nameof(Login));

            var viewModel = _accountViewService.GetManagePage(user);
            viewModel.ActionUrl = "/api/mediablob";
            viewModel.SearchUrl = "/api/mediasearch/blobs";
            var aggResponse = await _mediaSearchService.GetAggregates(this.CreateMediasAggregateQuery());
            viewModel.Filters = _accountViewService.CreateMediaNavigationFilters(aggResponse);
            return View(viewModel);
        }
        #endregion
        #region Settings
        [HttpGet]
        [RedirectUnauthenticatedRoute(url = _unauthenticatedRedirectUrl)]
        public async Task<IActionResult> Settings(string returnUrl = null)
        {
            var user = await GetUserWithProfile(this.User);
            if (user == null) return RedirectToAction(nameof(Login));

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
        //private string GetTarget(AccountUser obj) => obj.threadId ?? obj.channelId;
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
            obj.Logins.Clear();
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

                if (user == null)
                {
                    var viewModel = _accountViewService.GetForgotPasswordPage(model.Username);
                    model.Response = "There was an error resetting this users password";
                    return View(_accountFormRazorFile, viewModel);
                }
                if (!(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    await SendAccountConfirmationEmail(user);
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                await SendResetPasswordEmail(user);
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
        private async Task SendResetPasswordEmail(AccountUser user)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = ResetPasswordCallbackLink(user.Id.ToString(), code, Request.Scheme);
            await _emailSender.SendConfirmationLinkAsync(user, callbackUrl, "Reset password");
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
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordForm model)
        {
            var viewModel = _accountViewService.GetResetPasswordPage(model.UserId, model.Code);
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                viewModel.Form.Response = "There was an error resetting this users password, please contact support@parrapp.com";
                return View(_accountFormRazorFile, viewModel);
            }
            if (model.Password != model.ConfirmPassword)
            {
                viewModel.Form.Response = "Passwords do not match";
                return View(_accountFormRazorFile, viewModel);
            }

            var decodedCode = HttpUtility.HtmlDecode(model.Code);
           
            var result = await _userManager.ResetPasswordAsync(user, decodedCode, model.Password);

            if(result.Succeeded)
            {
                await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);
                return RedirectToAction(nameof(Profile));
            }

            viewModel.Form.Response = string.Join(",", result.Errors.Select(x => x.Description));
            
            return View(model);
        }
        #endregion
        #region Login
        [HttpGet]
        [AllowAnonymous]
        [RedirectAuthenticatedRoute(url = _authenticatedRedirectUrl)]
        public async Task<IActionResult> Login(string? username = null, string? response = null, string? message = null, string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ViewData["ReturnUrl"] = returnUrl; 
            var viewModel = await _accountViewService.GetLoginPage(username);
            viewModel.Form.Response = response;
            viewModel.Message = message;
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
            
            // This does not count login failures towards account lockout
            // To enable password failures to trigger account lockout,
            // set lockoutOnFailure: true
            var user = IsEmail(request.Username) ? await _userManager.FindByEmailAsync(request.Username) : await _userManager.FindByNameAsync(request.Username);

            var result = user != null
                ? await _signInManager.PasswordSignInAsync(user.UserName, request.Password, request.RememberMe.GetValueOrDefault(), lockoutOnFailure: false)
                : null;
            if (result == null)
            {
                var response = "No account with that email/username could be found";
                viewModel.Form.Response = response;
                return RedirectToAction(nameof(Login), new { username = request.Username, response = response });

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
                var response = "Invalid login attempt";
                viewModel.Form.Response = response;
                return RedirectToAction(nameof(Login), new { username = request.Username, response = response  }); 
            }
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

        #region Social Login

        [HttpPost]
        [AllowAnonymous]
        [RedirectAuthenticatedRoute(url = _authenticatedRedirectUrl)]
        //[ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }
      
        [HttpGet]
        [AllowAnonymous]
        [RedirectAuthenticatedRoute(url = _authenticatedRedirectUrl)]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                var message = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(Login), new { response = message });
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            if (!info.Principal.Identity.IsAuthenticated)
            {
                var message = $"Unable to login with {info.LoginProvider}";
                return RedirectToAction(nameof(Login), new { message = message, returnUrl = returnUrl });
            }
            var accessToken = info.AuthenticationTokens.Single(f => f.Name == "access_token").Value;
            var tokenType = info.AuthenticationTokens.Single(f => f.Name == "token_type").Value;
            var expiryDate = info.AuthenticationTokens.Single(f => f.Name == "expires_at").Value;

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await this._userManager.FindByEmailAsync(email);
            // If user already exists - not deactivated
            if(user != null && user.Status == AccountUserStatus.Active)
            {
                var alreadyLinkedResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
                if(alreadyLinkedResult.Succeeded)
                {
                    var firstname = info.Principal.FindFirstValue(ClaimTypes.GivenName);
                    var lastname = info.Principal.FindFirstValue(ClaimTypes.Surname);
                    // Update user
                    if (user.Id == Guid.Empty || user?.AccountProfile?.Firstname != firstname || user?.AccountProfile?.Lastname != lastname)
                        await this.UpdateUserProfile(user, info);
                    var login = await _userManager.AddLoginAsync(user, info);
                    RedirectToLocal(returnUrl ?? _authenticatedRedirectUrl);
                }
                return RedirectToAction(nameof(ExternalLoginExistingUser), new { username = user.UserName, provider = info.LoginProvider });
            }
            // if user exists - deactivated
            else if (user != null && user.Status == AccountUserStatus.Deactivated)
            {
                return RedirectToAction(nameof(ExternalLoginDeactivatedUser), new { username = user.UserName });
            }
            // if user doens't exist
            else
            {
                var username = email.Split("@").FirstOrDefault();
                return RedirectToAction(nameof(ExternalLoginNewUser), new { username = username });
            }
            /*
             var result = await this.CreateAndLoginUserFromExternalLogin(info);
 
            if (result.Succeeded)
            {
                var login = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
                return RedirectToLocal(returnUrl ?? _authenticatedRedirectUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            { 
                var message = $"You don't have an account with {info.LoginProvider}";
                return RedirectToAction(nameof(Login), new { message = message, returnUrl = returnUrl });
            }
            */
        }

        #region ExternalLoginNewUser
        [HttpGet]
        [AllowAnonymous]
        [RedirectAuthenticatedRoute(url = _authenticatedRedirectUrl)]
        public async Task<IActionResult> ExternalLoginNewUser(string? username = null, string? response = null, string? message = null, string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process 
            ViewData["ReturnUrl"] = returnUrl;
            var viewModel = await _accountViewService.GetExternalLoginNewUser(username);
            viewModel.Form.Response = response;
            viewModel.Message = message;
            return View(_accountFormRazorFile, viewModel);
        }
        [HttpPost]
        [AllowAnonymous]
        [RedirectAuthenticatedRoute(url = _authenticatedRedirectUrl)]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginNewUser(string username, string returnUrl = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var tempPassword = Guid.NewGuid().ToString().ToUpper() + "lower";

            var accessToken = info.AuthenticationTokens.Single(f => f.Name == "access_token").Value;
            var tokenType = info.AuthenticationTokens.Single(f => f.Name == "token_type").Value;
            var expiryDate = info.AuthenticationTokens.Single(f => f.Name == "expires_at").Value;
            if(username.Length < 3 || username.Length > 20)
            {
                return RedirectToAction(nameof(ExternalLoginNewUser), new { username = username, message = "Username must be between 3 and 20 characters", returnUrl = returnUrl });
            }
            var userCreateResult = await this.CreateUser(username, email, Foundations.Account.Constants.Domains.Public, tempPassword);
            if (!userCreateResult.Succeeded)
            {
                var message = string.Join(',', userCreateResult.Errors.Select(x => x.Description));
                return RedirectToAction(nameof(ExternalLoginNewUser), new { username = username, message = message, returnUrl = returnUrl });
            }
            var user = await this._userManager.FindByEmailAsync(email);
            await UpdateUserProfile(user, info);

            var login = await _userManager.AddLoginAsync(user, info);
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
          
            if (result.Succeeded)
            {
                //var login = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
                return RedirectToLocal(returnUrl ?? _authenticatedRedirectUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(ExternalLoginNewUser));
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                //ViewData["ReturnUrl"] = returnUrl;
                //ViewData["LoginProvider"] = info.LoginProvider;
                var message = $"You don't have an account with {info.LoginProvider}";
                return RedirectToAction(nameof(ExternalLoginNewUser), new { message = message, returnUrl = returnUrl });
            }

        }
        #endregion

        #region ExternalLoginExistingUser
        [HttpGet]
        [AllowAnonymous]
        [RedirectAuthenticatedRoute(url = _authenticatedRedirectUrl)]
        public async Task<IActionResult> ExternalLoginExistingUser(string? username = null, string? provider = null, string? response = null, string? message = null, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var viewModel = await _accountViewService.GetExternalLoginExistingUser(username, provider);
            viewModel.Form.Response = response;
            viewModel.Message = message;
            return View(_accountFormRazorFile, viewModel);
        }
        [HttpPost]
        [AllowAnonymous]
        [RedirectAuthenticatedRoute(url = _authenticatedRedirectUrl)]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginExistingUser(string username, string returnUrl = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var tempPassword = Guid.NewGuid().ToString().ToUpper() + "lower";
            var user = await this._userManager.FindByEmailAsync(email);
            var login = await _userManager.AddLoginAsync(user, info);

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            return HandleLoginResponse(result, returnUrl, info.LoginProvider);
        }
        #endregion

        private IActionResult HandleLoginResponse(Microsoft.AspNetCore.Identity.SignInResult? result, string returnUrl, string loginProvider)
        {
            if (result.Succeeded)
            {
                //var login = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
                return RedirectToLocal(returnUrl ?? _authenticatedRedirectUrl);
            }
            else if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else if (result.IsNotAllowed)
            {
                var message = $"You are not allowed to login with {loginProvider}";
                return RedirectToAction(nameof(Login), new { message = message, returnUrl = returnUrl });
            }
            else
            {
                var message = $"You require a one-time code to login";
                return RedirectToAction(nameof(Login), new { message = message, returnUrl = returnUrl });
            }
        }

        #region ExternalLoginDeactivatedUser
        [HttpGet]
        [AllowAnonymous]
        [RedirectAuthenticatedRoute(url = _authenticatedRedirectUrl)]
        public async Task<IActionResult> ExternalLoginDeactivatedUser(string? username = null, string? response = null, string? message = null, string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            ViewData["ReturnUrl"] = returnUrl;
            var viewModel = await _accountViewService.GetExternalLoginDeactivatedUser(username);
            viewModel.Form.Response = response;
            viewModel.Message = message;
            return View(_accountFormRazorFile, viewModel);
        }
        [HttpPost]
        [AllowAnonymous]
        [RedirectAuthenticatedRoute(url = _authenticatedRedirectUrl)]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginDeactivatedUser(string provider, string returnUrl = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var tempPassword = Guid.NewGuid().ToString().ToUpper() + "lower";
            var user = await this._userManager.FindByEmailAsync(email);
            
            var login = await _userManager.AddLoginAsync(user, info);
            user = await ResetDeactivatedUserDetails(info, user, tempPassword);

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            return HandleLoginResponse(result, returnUrl, info.LoginProvider);

        }
        #endregion
        private async Task<AccountUser> UpdateUserProfile(AccountUser user, ExternalLoginInfo info)
        {
            var profile = user.AccountProfileId != Guid.Empty
                ? _profileService.Get(user.AccountProfileId)
                : null;
            var firstname = info.Principal.FindFirstValue(ClaimTypes.GivenName);
            var lastname = info.Principal.FindFirstValue(ClaimTypes.Surname);
            if (profile == null)
            {
                profile = new AccountProfile();
                profile.Firstname = firstname;
                profile.Lastname = lastname;
                var guid = await _profileService.CreateWithGuid(profile);
                if (guid == null) return user;

                user.AccountProfileId = guid.GetValueOrDefault();
                await _userManager.UpdateAsync(user);
                await PostUpdateSteps(user);
            }
            else
            {
                profile.Firstname = firstname;
                profile.Lastname = lastname;
                await _profileService.Update(profile);
                // TODO: only run if user id is null
                
                await PostUpdateSteps(user);
            }
            return user;
        }
        private async Task<AccountUser> ResetDeactivatedUserDetails(ExternalLoginInfo info, AccountUser user, string password)
        {
            await UpdateUserProfile(user, info);

            user.Status = AccountUserStatus.Active;
            await _userManager.UpdateAsync(user);

            await this.PostUpdateSteps(user);
            return user;
        }
        private async Task<Microsoft.AspNetCore.Identity.SignInResult> CreateAndLoginUserFromExternalLogin(ExternalLoginInfo info)
        {
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var tempPassword = Guid.NewGuid().ToString().ToUpper() + "lower";
            var user = await this._userManager.FindByEmailAsync(email);
            // If currently de-activated, update user with details again
            if(user != null && user.Status == AccountUserStatus.Deactivated)
            {
                user = await ResetDeactivatedUserDetails(info, user, tempPassword);
            }
            if(user == null)
            {
                var result = await this.CreateUser(email, email, Foundations.Account.Constants.Domains.Public, tempPassword);
                if (!result.Succeeded) {
                    var message = string.Join(',', result.Errors.Select(x => x.Description));
                    throw new Exception(message);
                }
                user = await this._userManager.FindByEmailAsync(email);
                await UpdateUserProfile(user, info);
            }
            var login = await _userManager.AddLoginAsync(user, info);
            return await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
        }
       
        [HttpGet]
        [AllowAnonymous]
        [RedirectAuthenticatedRoute(url = _authenticatedRedirectUrl)]
        public async Task<IActionResult> ExternalLoginLinkConfirmation()
        {
            var code = HttpContext.Request.Query["code"];

            if (string.IsNullOrEmpty(code))
            {
                var message = $"Invalid code, please try again";
                return RedirectToAction(nameof(Login), new { message = message });
            }
           
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }
           
            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Login));
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                var message = $"Unable to link this account with {info.LoginProvider}, please try another provider or create an account with email";
                return RedirectToAction(nameof(Login), new { message = message });
            }

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

            var existingUserName = await _userManager.FindByNameAsync(request.Username);
            if(existingUserName != null)
            {
                viewModel.Form.Response = "A user with that username already exists";
                return View(_accountFormRazorFile, viewModel);
            }
            var existingEmail = await _userManager.FindByEmailAsync(request.Email);
            if (existingEmail != null)
            {
                viewModel.Form.Response = "A user with that email already exists";
                return View(_accountFormRazorFile, viewModel);
            }

            //Create user
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await this.CreateUser(request.Username, request.Email, Foundations.Account.Constants.Domains.Public, request.Password);
                    if(result.Errors != null && result.Errors.Any())
                    {
                        viewModel.Form.Response = result.Errors.FirstOrDefault().Description;
                        StaticLoggingService.LogError(result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    var errors = ModelState.Select(x => x.Value?.Errors?.FirstOrDefault()?.ErrorMessage);
                    viewModel.Form.Response = string.Join(",", errors);
                }
            }
            catch (Exception ex)
            {
                viewModel.Form.Response = "There was an error processing your request";
                StaticLoggingService.LogError(ex);
                return View(_accountFormRazorFile, viewModel);
            }
            var user = await _userManager.FindByEmailAsync(request.Email);

            //Send Email
            try
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //TODO: Move this to the pipeline
                var callbackUrl = EmailConfirmationLink(user.UserName, code, Request.Scheme);
                await _emailSender.SendConfirmationLinkAsync(user, user.Email, callbackUrl);
            }
            catch(Exception ex)
            {
                viewModel.Form.Response = "There was an send you the confirmation email, we will try again shortly";
                StaticLoggingService.LogError(ex);
            }
            await _signInManager.PasswordSignInAsync(user, request.Password, isPersistent: false, lockoutOnFailure: false);
            //Run pipeline
            await PostCreateSteps(user);
            StaticLoggingService.Log("User created a new account with password.");

            return RedirectToLocal(returnUrl ?? _authenticatedRedirectUrl);

        }
        #endregion

        #region Create User Account
        private async Task<IdentityResult> CreateUser(string username, string email, string domain, string password)
        {
            var user = new AccountUser { UserName = username, Email = email, Domain = domain, Status = AccountUserStatus.Active };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                // Create profile
                var profileId = await _profileService.CreateWithGuid(new AccountProfile() { UserId = user.Id });
                // Get user and update with profile id
                user = await _userManager.FindByNameAsync(username);
                user.AccountProfileId = profileId.GetValueOrDefault();
                await _userManager.UpdateAsync(user);
                await PostCreateSteps(user);
            }
            return result;
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
                await SendAccountConfirmationEmail(user);
                return RedirectToAction(nameof(AccountEmailConfirmation));
            }

            // If execution got this far, something failed, redisplay the form.
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        [RedirectAuthenticatedRoute(url = _authenticatedRedirectUrl)]
        public async Task<IActionResult> AccountEmailConfirmation(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            //ViewData["ReturnUrl"] = returnUrl;
            //var viewModel = await this.GetLoginViewModel();
            var viewModel = _accountViewService.GetForgotPasswordConfirmationPage();
            viewModel.Form.Response = "Please check your email to confirm your account";
            return View(_accountFormRazorFile, viewModel);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AccountConfirmation(string userId = null, string code = null)
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
            var user = await _userManager.FindByIdAsync(userId);
            var decodedCode = HttpUtility.HtmlDecode(code);
            var result = await _userManager.ConfirmEmailAsync(user, decodedCode);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction(nameof(Profile));
            }

            viewModel.Form.Response = string.Join(",", result.Errors.Select(x => x.Description));

            return View(_accountFormRazorFile, viewModel);
        }
        public string AccountConfirmationCallbackLink(string userId, string code, string scheme)
        {
            return this.Url.Action(
                action: nameof(AccountConfirmation),
                controller: ControllerHelper.NameOf<AccountController>(),
                values: new { userId, code },
                protocol: scheme);
        }

        private async Task SendAccountConfirmationEmail(AccountUser user)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = AccountConfirmationCallbackLink(user.Id.ToString(), code, Request.Scheme);
            await _emailSender.SendConfirmationLinkAsync(user, callbackUrl, "Confirm Account");
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

using Accelerate.Foundations.Users.Attributes;
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Aggregations;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading;
using Twilio.TwiML.Voice;
using static MassTransit.ValidationResultExtensions;
using Accelerate.Features.Onboarding.Services;
using Accelerate.Features.Onboarding.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Accelerate.Foundations.Users.Services;
using Twilio.TwiML.Messaging;
using Accelerate.Foundations.Communication.Services;
using Accelerate.Foundations.Mediator.Queries;
using MediatR;
using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Mediator.Commands;
using System.Data;
using Accelerate.Features.Content.Hydrators;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Accelerate.Features.Content.Controllers
{
    public static class OnboardingSessions
    {
        public static Dictionary<string, SignUpFormSessionData> _sessions { get; set; }
        public static Dictionary<string, SignUpFormSessionData> Sessions
        {
            get
            {
                if (_sessions == null) _sessions = new Dictionary<string, SignUpFormSessionData>();
                return _sessions;
            }
            set
            {
                _sessions = value;
            }
        }
    }
    public class OnboardingController : BaseController
    {
        private readonly IMediator _mediator;
        private SignInManager<UsersUser> _signInManager;
        private IEntityService<AccountsBusinessEntity> _businessService;
        private IEntityService<AccountsIndividualEntity> _individualService;
        IOnboardingContentService _contentService;
        IUsersUserService _userService;
        IMessageService _messageService;
        private const string _notFoundRazorFile = "~/Views/Threads/NotFound.cshtml";
        private const string signUpFormRazor = "~/Views/Shared/SignUp.cshtml";
        public OnboardingController(
            IMediator mediator,
            IMetaContentService metaContentService,
            IUsersUserService userService,
            IMessageService messageService,
            IEntityService<AccountsBusinessEntity> businessService,
            IEntityService<AccountsIndividualEntity> individualService,
            SignInManager<UsersUser> signInManager,
            IOnboardingContentService contentService) : base(metaContentService)
        {
            _contentService = contentService;
            _userService = userService;
            _messageService = messageService;
            _signInManager = signInManager;
            _businessService = businessService;
            _individualService = individualService;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        private AccountsBusinessEntity? GetUserBusiness(Guid userId)
        {
            return _businessService.Find(x => x.UserId == userId).FirstOrDefault();
        }
        private AccountsIndividualEntity? GetUserIndividual(Guid userId)
        {
            return _individualService.Find(x => x.UserId == userId).FirstOrDefault();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return RedirectToAction(nameof(SignUp));
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> SignUp()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var user = await _userService.FindByClaimAsync(this.User);
                if(user != null)
                {
                    var individualAccount = GetUserIndividual(user.Id);
                    var businessAccount = GetUserBusiness(user.Id);
                    if (individualAccount == null && businessAccount == null)
                    {
                        return RedirectToAction(nameof(IdentityCheck), new { userId = user.Id });
                    }
                    await _signInManager.SignOutAsync();
                }
            }
            var viewModel = await _contentService.CreateSignUpPage(this.User);

            return View(viewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpFormData formData)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var user = await _userService.FindByClaimAsync(this.User);
                return RedirectToAction(nameof(IdentityCheck));
            }

            if (formData.CustomerType == "Individual")
            {
                return RedirectToAction(nameof(ConsumerSignUp));
            }

            return RedirectToAction(nameof(BusinessSignUp));
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ConsumerSignUp(string? message)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(IdentityCheck), new { kycId = Guid.Empty });
            }

            var viewModel = await _contentService.CreateConsumerSignUpPage(this.User);
            viewModel.Message = message;
            return View(signUpFormRazor, viewModel);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> BusinessSignUp(string? message)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(IdentityCheck), new { kycId = Guid.Empty });
            }

            var viewModel = await _contentService.CreateBusinessSignUpPage(this.User);
            viewModel.Message = message;
            return View(signUpFormRazor, viewModel);
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ConsumerSignUp(SignUpFormDataConsumer formData)
        {

            // If user logged in, skip to the identity check
            if (this.User.Identity.IsAuthenticated) return RedirectToAction(nameof(IdentityCheck), new { kycId = Guid.Empty });
            // If user exists, tell them to login
            if (await _userService.FindByEmailAsync(formData.Email) != null) return RedirectToAction(nameof(BusinessSignUp), new { message = "A user already exists with that email, try login instead" });

            var user = await this.CreateUser(formData, Foundations.Accounts.Constants.Roles.UserAccountBusinessName);

            var provider = "Email";
            try
            {
                // Set session data
                SetSessionData(user, formData);

                // Send Email OTP
                var code = await _userService.GenerateTwoFactorTokenAsync(user, provider);
                var message = "Your security code is: " + code;
                if (provider == "Email") await _messageService.SendEmailAsync(user.Email, "Security Code", message);
              
                return RedirectToAction(nameof(AuthenticateOtp), new { userId = user.Id, provider });

            }
            catch (Exception ex)
            {
                // Delete the user
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
                var deleteResult = await _userService.Delete(user);
                if (deleteResult == 0)
                {
                    Foundations.Common.Services.StaticLoggingService.LogError($"Error deleting user: Email={user?.Email}, ID={user?.Id}");
                }
            }
            return RedirectToAction(nameof(ConsumerSignUp), new { message = "There was an error creating your account, please contact support" });

        }


        private async Task<UsersUser> CreateUser(SignUpFormDataConsumer formData, string userRole = Foundations.Accounts.Constants.Roles.UserAccountBusinessName)
        {
            // Create user
            var tempPassword = Guid.NewGuid().ToString().ToUpper() + DateTime.Now.ToShortTimeString();

            var result = await this._userService.CreateUser(formData.Email, formData.Email, Foundations.Users.Constants.Domains.Public, tempPassword);

            var user = await _userService.FindByEmailAsync(formData.Email);
            
            var roleResult = await this._userService.AddUserToRole(user, userRole);

            var passwordLogin = await _signInManager.PasswordSignInAsync(user, tempPassword, isPersistent: false, false);
            return user;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> BusinessSignUp(SignUpFormDataBusiness formData)
        {
            // If user logged in, skip to the identity check
            if (this.User.Identity.IsAuthenticated) return RedirectToAction(nameof(IdentityCheck), new { kycId = Guid.Empty });
            // If user exists, tell them to login
            if (await _userService.FindByEmailAsync(formData.Email) != null) return RedirectToAction(nameof(BusinessSignUp), new { message = "A user already exists with that email, try login instead" });

            var user = await this.CreateUser(formData, Foundations.Accounts.Constants.Roles.UserAccountBusinessName);

            var provider = "Email";
            try
            {
                // Set session data
                SetSessionData(user, formData);

                // Send Email OTP
                var code = await _userService.GenerateTwoFactorTokenAsync(user, provider);
                var message = "Your security code is: " + code;
                if (provider == "Email")  await _messageService.SendEmailAsync(user.Email, "Security Code", message);
                /*
                else if (provider == "Phone")
                {
                    //await _messageService.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
                }
                */

                return RedirectToAction(nameof(AuthenticateOtp), new { userId = user.Id, provider });

            }
            catch (Exception ex)
            {
                // Delete the user
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
                var deleteResult = await _userService.Delete(user);
                if (deleteResult == 0){
                    Foundations.Common.Services.StaticLoggingService.LogError($"Error deleting user: Email={user?.Email}, ID={user?.Id}");
                }
            }
            return RedirectToAction(nameof(BusinessSignUp), new { message = "There was an error creating your account, please contact support" });

        }
        private void SetSessionData(UsersUser user, SignUpFormDataConsumer formData)
        {
            // Set Session Data
            var data = OnboardingSessions.Sessions.ContainsKey(user.Id.ToString())
                ? OnboardingSessions.Sessions[user.Id.ToString()]
                : new SignUpFormSessionData();
            formData.Hydrate(data);
            data.CustomerType = "Individual";
            if (OnboardingSessions.Sessions.ContainsKey(user.Id.ToString()))
            {
                OnboardingSessions.Sessions[user.Id.ToString()] = data;
            }
            else
            {
                OnboardingSessions.Sessions.Add(user.Id.ToString(), data);
            }
        }
        private void SetSessionData(UsersUser user, SignUpFormDataBusiness formData)
        {
            // Set Session Data
            var data = OnboardingSessions.Sessions.ContainsKey(user.Id.ToString())
                ? OnboardingSessions.Sessions[user.Id.ToString()]
                : new SignUpFormSessionData();
            formData.Hydrate(data);
            data.CustomerType = "Business";
            if (OnboardingSessions.Sessions.ContainsKey(user.Id.ToString()))
            {
                OnboardingSessions.Sessions[user.Id.ToString()] = data;
            }
            else
            {
                OnboardingSessions.Sessions.Add(user.Id.ToString(), data);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> AuthenticateOtp(string provider, Guid? userId = null, string message = null)
        {
            if (!this.User.Identity.IsAuthenticated && userId == null)
            {
                return RedirectToAction(nameof(IdentityCheck), new { kycId = Guid.Empty });
            }

            var viewModel = userId != null
                ? await _contentService.CreateAuthenticateOtpPage(userId.GetValueOrDefault(), provider ?? "Email")
                : await _contentService.CreateAuthenticateOtpPage(this.User, provider ?? "Email");

            viewModel.Message = message;
            return View(viewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> AuthenticateOtp(SignUpAuthenticateOtpData formData)
        {
            // Validate OTP
            var result = await _signInManager.TwoFactorSignInAsync(formData.Provider, formData.Code, true, true);
            
            if (result.Succeeded)
            {
                // Retrieve data from session
                var sessionData = OnboardingSessions.Sessions[formData.UserId.ToString()];

                // Create Merchant
                var entity = new AccountsBusinessEntity()
                {
                    Name = sessionData.CompanyName,
                    Website = sessionData.Website,
                    UserId = formData.UserId,
                };

                var command = new CreateEntityCommand<AccountsBusinessEntity>() { Entity = entity };
                var response = await _mediator.Send(command);

                return Redirect(Foundations.Common.Constants.Paths.ProfilePath);
            }
            if (result.IsLockedOut)
            {
                Foundations.Common.Services.StaticLoggingService.Log("User account locked out.");
                return View(Foundations.Common.Constants.Paths.LockedPath);
            }
            else
            {
                return RedirectToAction(nameof(AuthenticateOtp), new { message = "Invalid code." });
            }

            return null;
        }


        [HttpGet]
        public async Task<IActionResult> IdentityCheck(Guid? userId)
        {
            var viewModel = userId.HasValue
                ? await _contentService.CreateIdentityCheckPage(userId.GetValueOrDefault())
                : await _contentService.CreateIdentityCheckPage(this.User);

            return View(viewModel);
        }
        /*
        [AllowAnonymous]
        [HttpGet("{*.}")]
        public async Task<IActionResult> NotFound()
        {
            return RedirectToAction(nameof(Index));
        }
        */
    }
}
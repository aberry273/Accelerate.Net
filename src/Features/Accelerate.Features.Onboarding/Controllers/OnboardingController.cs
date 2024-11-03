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
using Org.BouncyCastle.Crypto;
using Accelerate.Foundations.Portal.Services;

namespace Accelerate.Features.Content.Controllers
{
    public class OnboardingController : BaseController
    {
        private readonly IMediator _mediator;
        private SignInManager<UsersUser> _signInManager;
        private IEntityService<AccountsBusinessEntity> _businessService;
        private IEntityService<AccountsIndividualEntity> _individualService;
        IPortalSessionService _portalSessionService;
        IOnboardingContentService _contentService;
        IUsersUserService _userService;
        IMessageService _messageService;
        private const string _notFoundRazorFile = "~/Views/Threads/NotFound.cshtml";
        private const string signUpFormRazor = "~/Views/Shared/SignUp.cshtml";
        private const string finalizeFormRazor = "~/Views/Shared/Finalize.cshtml";
        private const string _accountFinalizedPath = "/Profile";
        public OnboardingController(
            IMediator mediator,
            IMetaContentService metaContentService,
            IPortalSessionService portalSessionService,
            IUsersUserService userService,
            IMessageService messageService,
            IEntityService<AccountsBusinessEntity> businessService,
            IEntityService<AccountsIndividualEntity> individualService,
            SignInManager<UsersUser> signInManager,
            IOnboardingContentService contentService) : base(metaContentService)
        {
            _contentService = contentService;
            _userService = userService;
            _portalSessionService = portalSessionService;
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
            if (this.User.Identity.IsAuthenticated)
            {
                var user = await _userService.FindByClaimAsync(this.User);
                if (user == null)
                {
                    await _signInManager.SignOutAsync();
                    return RedirectToAction(nameof(SignUp));
                }

                if (await _userService.UserInRole(user, Foundations.Portal.Constants.Roles.AccountIndividual))
                {
                    // Check if user has an individual account, if so redirect to logged in path, if not redirect to finalize step
                    var individualAccount = GetUserIndividual(user.Id);
                    if (individualAccount != null)
                    {
                        _portalSessionService.SetAccountSession(individualAccount.Id);
                        return Redirect(_accountFinalizedPath);
                    }
                    return RedirectToAction(nameof(FinalizeIndividual));
                }
                else if (await _userService.UserInRole(user, Foundations.Portal.Constants.Roles.AccountBusiness))
                {
                    // Check if user has an business account, if so redirect to logged in path, if not redirect to finalize step
                    var businessAccount = GetUserBusiness(user.Id);
                    if (businessAccount != null)
                    {
                        _portalSessionService.SetAccountSession(businessAccount.Id);
                        return Redirect(_accountFinalizedPath);
                    }
                    return RedirectToAction(nameof(FinalizeBusiness));
                }
                await _signInManager.SignOutAsync();
            }
            return RedirectToAction(nameof(SignUp));
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> SignUp()
        {
            if (this.User.Identity.IsAuthenticated) return RedirectToAction(nameof(Index));

            var viewModel = await _contentService.CreateSignUpPage(this.User);

            return View(viewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpFormRequest formData)
        {
            if (this.User.Identity.IsAuthenticated) return RedirectToAction(nameof(Index));

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
            if (this.User.Identity.IsAuthenticated) return RedirectToAction(nameof(Index));

            var viewModel = await _contentService.CreateConsumerSignUpPage(this.User);
            viewModel.Message = message;
            return View(signUpFormRazor, viewModel);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> BusinessSignUp(string? message)
        {
            if (this.User.Identity.IsAuthenticated) return RedirectToAction(nameof(Index));
             
            var viewModel = await _contentService.CreateBusinessSignUpPage(this.User);
            viewModel.Message = message;
            return View(signUpFormRazor, viewModel);
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ConsumerSignUp(SignUpFormAccountRequest formData)
        {

            // If user logged in, skip to the identity check
            if (this.User.Identity.IsAuthenticated) return RedirectToAction(nameof(Index));
            // If user exists, tell them to login
            if (await _userService.FindByEmailAsync(formData.Email) != null) return RedirectToAction(nameof(BusinessSignUp), new { message = "A user already exists with that email, try login instead" });

            var user = await this.CreateUser(formData, Foundations.Portal.Constants.Roles.AccountIndividual);
             
            try
            {
                // Send Email OTP
                await SendEmailCode(user);
                return RedirectToAction(nameof(AuthenticateOtp), new { userId = user.Id, TokenOptions.DefaultEmailProvider });

            }
            catch (Exception ex)
            {
                // Delete the user
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
                if (await _userService.Delete(user) == 0) {
                    Foundations.Common.Services.StaticLoggingService.LogError($"Error deleting user: Email={user?.Email}, ID={user?.Id}");
                }
            }
            return RedirectToAction(nameof(ConsumerSignUp), new { message = "There was an error creating your account, please contact support" });

        }

        private async System.Threading.Tasks.Task SendEmailCode(UsersUser user)
        {
            var code = await _userService.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
            var message = "Your security code is: " + code;
            await _messageService.SendEmailAsync(user.Email, "Security Code", message);
            //await _messageService.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
        }


        private async Task<UsersUser> CreateUser(SignUpFormAccountRequest formData, string userRole)
        {
            var tempPassword = Guid.NewGuid().ToString().ToUpper() + DateTime.Now.ToShortTimeString();

            // Create user
            var result = await this._userService.CreateUser(formData.Email, formData.Email, Foundations.Users.Constants.Domains.Public, tempPassword);

            var user = await _userService.FindByEmailAsync(formData.Email);
          
            var roleResult = await this._userService.AddUserToRole(user, userRole);

            return user;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> BusinessSignUp(SignUpFormAccountRequest formData)
        {
            // If user logged in, skip to the identity check
            if (this.User.Identity.IsAuthenticated) return RedirectToAction(nameof(Index));
            // If user exists, tell them to login
            if (await _userService.FindByEmailAsync(formData.Email) != null) return RedirectToAction(nameof(BusinessSignUp), new { message = "A user already exists with that email, try login instead" });

            var user = await this.CreateUser(formData, Foundations.Portal.Constants.Roles.AccountBusiness);

            try
            {
                // Send Email OTP
                await SendEmailCode(user);
                return RedirectToAction(nameof(AuthenticateOtp), new { userId = user.Id, TokenOptions.DefaultEmailProvider });

            }
            catch (Exception ex)
            {
                // Delete the user
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
                if (await _userService.Delete(user) == 0)
                {
                    Foundations.Common.Services.StaticLoggingService.LogError($"Error deleting user: Email={user?.Email}, ID={user?.Id}");
                }
            }
           
            return RedirectToAction(nameof(BusinessSignUp), new { message = "There was an error creating your account, please contact support" });

        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> AuthenticateOtp(string provider, Guid? userId = null, string message = null)
        {
            // If user logged in, skip to the identity check
            if ( this.User.Identity.IsAuthenticated) return RedirectToAction(nameof(Index));
            // If everything is null - user hits this page randomly redirec to index to standard flow
            if (!this.User.Identity.IsAuthenticated && userId == null && provider == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var viewModel = this.User.Identity.IsAuthenticated
                ? await _contentService.CreateAuthenticateOtpPage(this.User, provider ?? "Email")
                : await _contentService.CreateAuthenticateOtpPage(userId.GetValueOrDefault(), provider ?? "Email");

            viewModel.Message = message;
            return View(viewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> AuthenticateOtp(SignUpAuthenticateOtpData formData)
        {
            // Validate OTP
            var user = await _userService.FindByIdAsync(formData.UserId.ToString());

            var verified = await _userService.VerifyTwoFactorTokenAsync(user, "Email", formData.Code);
            
            if (!verified)
            {
                return RedirectToAction(nameof(AuthenticateOtp), new { message = "Unable to verify your account. Please contact support." });
            }

            var confirmedEmail = await _userService.ConfirmEmailAsync(user);
            await _signInManager.SignInAsync(user, isPersistent: false, authenticationMethod: null);


            if (await _userService.UserInRole(user, Foundations.Portal.Constants.Roles.AccountIndividual))
            {
                return RedirectToAction(nameof(FinalizeIndividual));
            }
            else if (await _userService.UserInRole(user, Foundations.Portal.Constants.Roles.AccountBusiness))
            {
                return RedirectToAction(nameof(FinalizeBusiness));
            }

            return RedirectToAction(nameof(AuthenticateOtp), new { message = "Unable to verify your account. Please contact support." });
        }

        [HttpGet]
        public async Task<IActionResult> FinalizeIndividual(string? message = null)
        {
            UsersUser user = (this.User.Identity.IsAuthenticated)
                ? await _userService.FindByClaimAsync(this.User)
                : null;
            if (user == null) {
                await _signInManager.SignOutAsync();
                return RedirectToAction(nameof(SignUp));
            }
            var viewModel = await _contentService.CreateFinalizeIndividualAccountPage(user);
            viewModel.Message = message;
            return View(finalizeFormRazor, viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> FinalizeBusiness(string? message = null)
        {
            UsersUser user = (this.User.Identity.IsAuthenticated)
                ? await _userService.FindByClaimAsync(this.User)
                : null;

            if (user == null) {
                await _signInManager.SignOutAsync();
                return RedirectToAction(nameof(Index));
            }

            var viewModel = await _contentService.CreateFinalizeBusinessAccountPage(user);
            viewModel.Message = message;
            return View(finalizeFormRazor, viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> FinalizeIndividual(FinalizeFormRequestIndividual formData)
        {
            UsersUser user = (this.User.Identity.IsAuthenticated)
                ? await _userService.FindByClaimAsync(this.User)
                : null;

            if (user == null)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction(nameof(Index));
            }
            // IF account already exists in session, redirect out of onboarding controller
            if (!string.IsNullOrEmpty(_portalSessionService.TryGetSelectedAccountId()))
            {
                Redirect(_accountFinalizedPath);
            }

            // Create individual
            var entity = new AccountsIndividualEntity()
            {
                Firstname = formData.Firstname,
                Lastname = formData.Lastname,
                Email = formData.Email,
                TaxId = formData.TaxId,
                UserId = user.Id,
                CountryCode = formData.Country
            };

            var command = new CreateEntityCommand<AccountsIndividualEntity>() { Entity = entity };
            var response = await _mediator.Send(command);

            if(!response.Success)
            {
                return RedirectToAction(nameof(FinalizeIndividual), new { message = "Unable to finalize your account. Please contact support." });
            }
            // Add user to account created access role
            var roleResult = await this._userService.AddUserToRole(user, Foundations.Portal.Constants.Roles.AccountCreated);
            // TODO: Have command return ID generated so this isn't a chance / race condition
            var account = _businessService.Find(x => x.UserId == user.Id).FirstOrDefault();
            if (account != null) _portalSessionService.SetAccountSession(account.Id);

            return Redirect(_accountFinalizedPath);
        }
        [HttpPost]
        public async Task<IActionResult> FinalizeBusiness(FinalizeFormRequestBusiness formData)
        {
            UsersUser user = (this.User.Identity.IsAuthenticated)
                ? await _userService.FindByClaimAsync(this.User)
                : null;

            if (user == null)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction(nameof(Index));
            }
            // IF account already exists in session, redirect out of onboarding controller
            if (!string.IsNullOrEmpty(_portalSessionService.TryGetSelectedAccountId()))
            {
                Redirect(_accountFinalizedPath);
            }

            // Create consumer
            var entity = new AccountsBusinessEntity()
            {
                Name = formData.CompanyName,
                Website = formData.Website,
                TaxId = formData.TaxId,
                UserId = user.Id,
                CountryCode = formData.Country,
                Type = Enum.Parse<BusinessAccountType>(formData.AccountType),
            };

            var command = new CreateEntityCommand<AccountsBusinessEntity>() { Entity = entity };
            var response = await _mediator.Send(command);

            if (!response.Success)
            {
                return RedirectToAction(nameof(FinalizeBusiness), new { message = "Unable to finalize your account. Please contact support." });
            }
            // Add user to account created access role
            var roleResult = await this._userService.AddUserToRole(user, Foundations.Portal.Constants.Roles.AccountCreated);
            
            // TODO: Have command return ID generated so this isn't a chance / race condition
            var account = _individualService.Find(x => x.UserId == user.Id).FirstOrDefault();
            if(account != null) _portalSessionService.SetAccountSession(account.Id);

            return Redirect(_accountFinalizedPath);
        }
    }
}
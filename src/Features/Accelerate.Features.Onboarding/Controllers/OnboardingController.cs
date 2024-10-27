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

namespace Accelerate.Features.Content.Controllers
{
    public class OnboardingController : BaseController
    {
        SignInManager<UsersUser> _signInManager;
        IOnboardingContentService _contentService;
        private const string _notFoundRazorFile = "~/Views/Threads/NotFound.cshtml";
        public OnboardingController(
            IMetaContentService metaContentService,
            SignInManager<UsersUser> signInManager,
            IEntityService<UsersProfile> profileService,
            IOnboardingContentService contentService,
            UserManager<UsersUser> userManager) : base(metaContentService)
        {
            _signInManager = signInManager;
            _contentService = contentService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return RedirectToAction(nameof(SignUp), new { kycId = Guid.Empty });
        }

        [HttpGet]
        public async Task<IActionResult> SignUp()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(IdentityCheck), new { kycId = Guid.Empty });
            }

            var viewModel = await _contentService.CreateSignUpPage(this.User);

            return View(viewModel);
        }

        [RedirectUnauthenticatedRoute(url = Foundations.Users.Constants.Paths.UnauthenticatedRedirectUrl)]
        [HttpGet]
        public async Task<IActionResult> IdentityCheck(Guid? kycId)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return Redirect(Foundations.Users.Constants.Paths.UnauthenticatedRedirectUrl);
                //return View(_contentViewService.CreateAnonymousListingPage());
            }

            var viewModel = await _contentService.CreateIdentityCheckPage(this.User);

            return View(viewModel);
        }
    }
}
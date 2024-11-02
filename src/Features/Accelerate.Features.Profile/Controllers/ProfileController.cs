using Accelerate.Foundations.Users.Attributes;
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Features.Profile.Models.Views;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Services;
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
using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Accelerate.Features.Profile.Models;
using Accelerate.Foundations.Users.Models;
using Accelerate.Features.Profile.Services;
using Elastic.Clients.Elasticsearch;
using Accelerate.Features.Profile.Models.Data;
using System.Security.Claims;
using Accelerate.Foundations.Common.Helpers;
using System.Text.Encodings.Web;
using System.Web;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Media.Models.Data;
using Accelerate.Foundations.Content.Services;
using Accelerate.Foundations.Common.Extensions;
using Twilio.TwiML.Messaging;
using Accelerate.Foundations.Content.Models.Entities;
using static MassTransit.ValidationResultExtensions;
using Accelerate.Foundations.Users.EventBus;
using Microsoft.AspNetCore.Mvc.Rendering;
using Accelerate.Foundations.Communication.Services;

namespace Accelerate.Features.Profile.Controllers
{
    //[Authorize]
    public class ProfileController : BaseController
    {
        private UserManager<UsersUser> _userManager;
        private IProfileViewService _accountViewService;
        private IEntityService<UsersProfile> _profileService;
        public ProfileController(
            IMetaContentService contentService,
            SignInManager<UsersUser> signInManager,
            UserManager<UsersUser> userManager,
            IEmailSender<UsersUser> emailSender,
            IProfileViewService accountViewService,
            IMessageService messageService,
            IEntityService<UsersProfile> profileService,
            Bind<IUsersBus, IPublishEndpoint> publishEndpoint,
            IElasticService<UsersUserDocument> searchService)
            : base(contentService)
        {
            _userManager = userManager;
            _profileService = profileService;
            _accountViewService = accountViewService;
            // Move to service
        }
         
        private const string _accountFormRazorFile = "~/Views/Profile/ProfileFormPage.cshtml";
        
        private async Task<UsersUser> GetUserWithProfile(ClaimsPrincipal principle)
        {
            var user = await _userManager.GetUserAsync(principle);
            if (user == null) return null;
            var profile = _profileService.Get(user.UsersProfileId.GetValueOrDefault());
            user.UsersProfile = profile;
            return user;
        }
        #region Profile
        [HttpGet]
        [RedirectUnauthenticatedRoute(url = Foundations.Common.Constants.Paths.LoginPath)]
        public async Task<IActionResult> Index(string returnUrl = null)
        {
            var user = await GetUserWithProfile(this.User);
            if (user == null) return Redirect(Foundations.Common.Constants.Paths.LoginPath);

            var viewModel = _accountViewService.GetManagePage(user);
            return View(viewModel);
        }
        #endregion
        
        #region Settings
        [HttpGet]
        [RedirectUnauthenticatedRoute(url = Foundations.Common.Constants.Paths.LoginPath)]
        public async Task<IActionResult> Settings(string returnUrl = null)
        {
            var user = await GetUserWithProfile(this.User);
            if (user == null) return Redirect(Foundations.Common.Constants.Paths.LoginPath);

            var viewModel = _accountViewService.GetManagePage(user);
            return View(viewModel);
        }
        #endregion  
    }
}

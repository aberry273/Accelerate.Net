using Accelerate.Foundations.Users.Attributes;
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading;
using Twilio.TwiML.Voice;
using Accelerate.Features.Transactions.Services;
using Accelerate.Foundations.Users.Services;
using Accelerate.Features.Transactions.Models.Views;
using Accelerate.Foundations.Portal.Services;

namespace Accelerate.Features.Content.Controllers
{
    [Route("[controller]")]
    public class BaseTransactionsController<T> : BaseController where T : IBaseEntity
    {
        protected SignInManager<UsersUser> _signInManager;
        IUsersUserService _userService;
        IEntityService<UsersProfile> _profileService;
        IMetaContentService _contentService;
        IPortalSessionService _portalSessionService;
        protected IEntityService<T> _entityService;
        protected ITransactionsBaseEntityViewService<T> _contentViewService;
        protected string _entityName;
        protected string _razorPath;
        private const string _notFoundRazorFile = "~/Views/Shared/NotFound.cshtml";
        public BaseTransactionsController(
            string entityName,
            SignInManager<UsersUser> signInManager,
            UserManager<UsersUser> userManager,
            IUsersUserService userService,
            IEntityService<UsersProfile> profileService,
            IPortalSessionService portalSessionService,
            IMetaContentService contentService,
            IEntityService<T> entityService,
            ITransactionsBaseEntityViewService<T> contentViewService) : base(contentService)
        {
            _entityName = entityName;
            _razorPath = $"~/Views/{_entityName}";
            _signInManager = signInManager;
            _userService = userService;
            _entityService = entityService;
            _profileService = profileService;
            _portalSessionService = portalSessionService;
            _contentService = contentService;
            _contentViewService = contentViewService;
        }

        [HttpGet]
        [RedirectUnauthenticatedRoute(url = Foundations.Users.Constants.Paths.UnauthenticatedRedirectUrl)]
        public virtual async Task<IActionResult> Index()
        {
            var user = await _userService.FindByClaimAsync(this.User);
            if (user == null) return RedirectToAction("NotFound", "Transactions");

            // get ID from session
            var id = _portalSessionService.TryGetSelectedAccountId();
            var guid = Guid.Parse(id);

            var item = _entityService.Get(guid);
            if (item == null)
            {
                return RedirectToAction(nameof(NotFound));
            }

            var viewModel = await _contentViewService.CreateEntityPage(user, item);

            return View(viewModel);
        }

        [HttpGet("Listing")]
        public async Task<IActionResult> Listing()
        {
            var user = await _userService.FindByClaimAsync(this.User);
            if (user == null) return RedirectToAction("Index", "Transactions");
          
            var items = _entityService.Find(x => true);
            
            var viewModel = await _contentViewService.CreateListPage(user, items);

            return View(viewModel);
        }
         
        [HttpGet]
        [Route("{id}")]
        [RedirectUnauthenticatedRoute(url = Foundations.Users.Constants.Paths.UnauthenticatedRedirectUrl)]
        public virtual async Task<IActionResult> Index([FromRoute] Guid id)
        {
            var user = await _userService.FindByClaimAsync(this.User);
            if (user == null) return RedirectToAction("Index", "Transactions");

            var item = _entityService.Get(id);
            if (item == null)
            {
                return RedirectToAction(nameof(NotFound));
            }

            var viewModel = await _contentViewService.CreateEntityPage(user, item);

            return View(viewModel);
        }
        
        [HttpGet("notfound")]
        public virtual async Task<IActionResult> NotFound()
        {
            var user = await _userService.FindByClaimAsync(this.User);
            if (user == null) return RedirectToAction("Index", "Transactions");

            var title = $"{_entityName} not found";
            var description = "We are unable to retrieve this page, it may have been deleted or made private.";
            var viewModel = await _contentViewService.CreateNotFoundPage(user, title, description);
            return View(_notFoundRazorFile, viewModel);
        }

        [Route("{id}/Edit")]
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] Guid id)
        {
            var user = await _userService.FindByClaimAsync(this.User);
            if (user == null) return RedirectToAction("Index", "Transactions");

            var item = _entityService.Get(id);
            if (item == null)
            {
                return RedirectToAction(nameof(NotFound));
            }

            var viewModel = await _contentViewService.CreateUpdatePage(user, item);
            return View(viewModel);
        }

        [Route("Create")]
        [HttpGet]
        public virtual async Task<IActionResult> Create()
        {
            var user = await _userService.FindByClaimAsync(this.User);
            if (user == null) return RedirectToAction("Index", "Transactions"); 

            var viewModel = await _contentViewService.CreateNewPage(user);

            return View($"{_razorPath}s/Create.cshtml", viewModel);
        }

        private string GetChannelView(string routeName)
        {
            return $"{_razorPath}/{routeName}.cshtml";
        }
    }
}
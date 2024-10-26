using Accelerate.Features.Content.Models.Views;
using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Users.Attributes;
using Accelerate.Foundations.Users.Models;
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Content.Services;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Aggregations;
using Elastic.Clients.Elasticsearch.QueryDsl;
using ImageMagick;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading;
using Twilio.TwiML.Voice;

namespace Accelerate.Features.Content.Controllers
{
    [Route("[controller]")]
    public class BaseContentController<T> : BaseController where T : ContentEntityDocument
    {
        protected SignInManager<UsersUser> _signInManager;
        UserManager<UsersUser> _userManager;
        IEntityService<UsersProfile> _profileService;
        IMetaContentService _contentService;
        protected IElasticService<T> _searchService;
        protected IBaseContentEntityViewService<T> _contentViewService;
        protected IElasticService<ContentPostDocument> _postSearchService;
        protected IContentPostElasticService _contentElasticSearchService;
        const string _unauthenticatedRedirectUrl = "/Account/login";
        protected string _entityName;
        protected string _razorPath;
        private const string _notFoundRazorFile = "~/Views/Shared/ContentNotFound.cshtml";
        public BaseContentController(
            string entityName,
            SignInManager<UsersUser> signInManager,
            UserManager<UsersUser> userManager,
            IEntityService<UsersProfile> profileService,
            IMetaContentService contentService,
            IElasticService<T> searchService,
            IBaseContentEntityViewService<T> contentViewService,
            IElasticService<ContentPostDocument> postSearchService,
            IContentPostElasticService contentElasticSearchService) : base(contentService)
        {
            _entityName = entityName;
            _razorPath = $"~/Views/{_entityName}";
            _signInManager = signInManager;
            _userManager = userManager;
            _profileService = profileService;
            _contentService = contentService;
            _searchService = searchService;
            _contentViewService = contentViewService;
            _postSearchService = postSearchService;
            _contentElasticSearchService = contentElasticSearchService;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual QueryDescriptor<T> BuildGetEntitiesQuery(UsersUser user)
        {
            var query = new RequestQuery();
            var filter = _searchService.FilterValues("userId", ElasticCondition.Filter, QueryOperator.Contains, new List<string>() { user.Id.ToString(), Foundations.Common.Constants.Global.GlobalAdminContent.ToString() }, true );
            query.Filters.Add(filter);

            return _searchService.CreateQuery(query);
        }

        protected async Task<UsersUser> GetUserWithProfile(ClaimsPrincipal principle)
        {
            var user = await _userManager.GetUserAsync(principle);
            if (user == null)
            {
                return null;
            }
            var profile = _profileService.Get(user.UsersProfileId.GetValueOrDefault());
            user.UsersProfile = profile;
            return user;
        }

        [HttpGet]
        public async Task<IActionResult> Landing()
        {
            var user = await GetUserWithProfile(this.User);
            if (user == null) return RedirectToAction("Index", "Account");

            var filters = new List<QueryFilter>()
            {
                _postSearchService.Filter(Foundations.Content.Constants.Fields.PostType, "Post")
            };
            var query = this.BuildGetEntitiesQuery(user);
            var items = await _searchService.Search(query, 0, 100);

            //var items = await _searchService.Search(GetUserItemsQuery(user), 0, 100);
            var aggResponse = await _postSearchService.GetAggregates(_contentElasticSearchService.CreateThreadAggregateQuery(filters));

            var viewModel = await _contentViewService.CreateAllPage(user, items, aggResponse);

            return View(viewModel);
        }

        protected async Task<ContentBasePage> CreateIndexPage(UsersUser user, T item)
        {
            var query = this.BuildGetEntitiesQuery(user);
            var channels = await _searchService.Search(query);
            var aggResponse = await _postSearchService.GetAggregates(_contentElasticSearchService.CreateChannelAggregateQuery(item.Id));

            var viewModel = await _contentViewService.CreateEntityPage(user, item, channels, aggResponse);
            viewModel.RouteName = "All";
            return viewModel;
        } 
        [HttpGet]
        [Route("{id}")]
        [RedirectUnauthenticatedRoute(url = _unauthenticatedRedirectUrl)]
        public virtual async Task<IActionResult> Index([FromRoute] Guid id)
        {
            var user = await GetUserWithProfile(this.User);
            if (user == null) return RedirectToAction("Index", "Account");

            var response = await _searchService.GetDocument<T>(id.ToString());
            var item = response.Source;

            if (item == null)
            {
                return RedirectToAction(nameof(NotFound));
            }

            var viewModel = await this.CreateIndexPage(user, item);
            return View($"{_razorPath}s/Index.cshtml", viewModel);
        }
        /*
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user = await GetUserWithProfile(this.User);
            var items = await _searchService.Search(GetUserItemsQuery(user));
            
            var viewModel = _contentViewService.CreateAddPage(user, items);
            return View(_notFoundRazorFile, viewModel);
        }
        */
        [HttpGet("notfound")]
        public virtual async Task<IActionResult> NotFound()
        {
            var user = await GetUserWithProfile(this.User);
            var title = $"{_entityName} not found";
            var description = "We are unable to retrieve this page, it may have been deleted or made private.";
            var viewModel = _contentViewService.CreateNotFoundPage(user, title, description);
            return View(_notFoundRazorFile, viewModel);
        }

        [Route("{id}/Edit")]
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] Guid id)
        {
            var user = await GetUserWithProfile(this.User);
            var channels = await _searchService.Search(BuildGetEntitiesQuery(user));
            var item = await _searchService.GetDocument<T>(id.ToString());
            var viewModel = _contentViewService.CreateEditPage(user, channels, item.Source);
            return View($"{_razorPath}s/Create.cshtml", viewModel);
        }
        [Route("Create")]
        [HttpGet]
        public virtual async Task<IActionResult> Create()
        {
            var user = await GetUserWithProfile(this.User);
            var channels = await _searchService.Search(BuildGetEntitiesQuery(user));

            var viewModel = _contentViewService.CreateAddPage(user, channels);
            return View($"{_razorPath}s/Create.cshtml", viewModel);
        }

        private string GetChannelView(string routeName)
        {
            return $"{_razorPath}/{routeName}.cshtml";
        }
    }
}
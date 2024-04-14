using Accelerate.Features.Content.Models.Views;
using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Account.Attributes;
using Accelerate.Foundations.Account.Models.Entities;
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
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Twilio.TwiML.Voice;

namespace Accelerate.Features.Content.Controllers
{
    public class ContentController : BaseController
    {
        UserManager<AccountUser> _userManager;
        IMetaContentService _contentService;
        IContentPostElasticService _contentElasticSearchService;
        IElasticService<ContentPostDocument> _postSearchService;
        IElasticService<ContentChannelDocument> _channelSearchService;
        IEntityService<AccountProfile> _profileService;
        IContentViewService _contentViewService;
        const string _unauthenticatedRedirectUrl = "/Account/login";
        private const string _notFoundRazorFile = "~/Views/Content/NotFound.cshtml";
        public ContentController(
            IMetaContentService service,
            IContentViewService contentViewService,
            IContentPostElasticService postElasticSearchService,
            IEntityService<ContentPostEntity> postService,
            IEntityService<AccountProfile> profileService,
            IElasticService<ContentPostDocument> searchService,
            IElasticService<ContentChannelDocument> channelService,
            UserManager<AccountUser> userManager) : base(service)
        {
            _userManager = userManager;
            _contentViewService = contentViewService;
            _contentService = service;
            _postSearchService = searchService;
            _profileService = profileService;
            _contentElasticSearchService = postElasticSearchService;
            _channelSearchService = channelService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return RedirectToAction("Browse");
        }

        private async Task<AccountUser> GetUserWithProfile(ClaimsPrincipal principle)
        {
            var user = await _userManager.GetUserAsync(principle);
            if (user == null) return null;
            var profile = _profileService.Get(user.AccountProfileId);
            user.AccountProfile = profile;
            return user;
        }
        [RedirectUnauthenticatedRoute(url = _unauthenticatedRedirectUrl)]
        public async Task<IActionResult> Browse()
        {
            var user = await GetUserWithProfile(this.User);
            if (user == null) return RedirectToAction("Index", "Account");

            var channels = await _channelSearchService.Search(GetUserChannelsQuery(user));

            var viewModel = _contentViewService.CreateChannelsPage(user, channels);

            return View(viewModel);
        }

        [RedirectUnauthenticatedRoute(url = _unauthenticatedRedirectUrl)]
        public async Task<IActionResult> Channel([FromRoute] Guid id)
        {
            var user = await GetUserWithProfile(this.User);
            if (user == null) return RedirectToAction("Index", "Account");

            var response = await _channelSearchService.GetDocument<ContentChannelDocument>(id.ToString());
            var item = response.Source;

            if (item == null) return RedirectToAction(nameof(ChannelNotFound));

            var channels = await _channelSearchService.Search(GetUserChannelsQuery(user));
            var aggResponse = await _postSearchService.GetAggregates(_contentElasticSearchService.CreateChannelAggregateQuery(item.Id));

            var viewModel = _contentViewService.CreateChannelPage(user, item, channels, aggResponse);
            return View(viewModel);
        }
        private QueryDescriptor<ContentChannelDocument> GetUserChannelsQuery(AccountUser user)
        {
            var query = new QueryDescriptor<ContentChannelDocument>();
            query.MatchAll();
            query.Term(x => x.UserId.Suffix("keyword"), user.Id.ToString());
            return query;
        }

        [HttpGet]
       // [Route("{id}")]
        [RedirectUnauthenticatedRoute(url = _unauthenticatedRedirectUrl)]
        public async Task<IActionResult> Thread([FromRoute] Guid id)
        {
            var user = await GetUserWithProfile(this.User);
            if (user == null) return RedirectToAction("Index", "Account");  
            var response = await _postSearchService.GetDocument<ContentPostDocument>(id.ToString());
            
            var item = response.Source;

            if (item == null) return RedirectToAction(nameof(PostNotFound));

            var replies = await _channelSearchService.Search(this._contentElasticSearchService.BuildRepliesSearchQuery(item.Id.ToString()), 0, 100);
            var aggResponse = await _postSearchService.GetAggregates(_contentElasticSearchService.CreateThreadAggregateQuery(item.Id));
            var viewModel = _contentViewService.CreateThreadPage(user, item, aggResponse, replies);
            //var parents = await _channelSearchService.GetDocuments<ContentPostDocument>(item.ParentIds);
            //viewModel.Parents = parents.IsValidResponse && parents.IsSuccess ? pare
            viewModel.Parents = new List<ContentPostDocument>();
            return View(viewModel);
        } 
         
        private QueryDescriptor<ContentPostDocument> GetRepliesQuery(ContentPostDocument item)
        {
            var query = new QueryDescriptor<ContentPostDocument>();
            query.MatchAll();
            query.Term(x => x.TargetThread.Suffix("keyword"), item.ThreadId.ToString());
            return query;
        }
        [HttpGet]
        public async Task<IActionResult> ChannelNotFound()
        {
            var user = await GetUserWithProfile(this.User);
            var title = "Channel not found";
            var description = "We are unable to retrieve this channel, this may have been deleted or made private.";
            var viewModel = _contentViewService.CreateNotFoundPage(user, title, description);
            return View(_notFoundRazorFile, viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> PostNotFound()
        {
            var user = await GetUserWithProfile(this.User);
            var title = "Post not found";
            var description = "We are unable to retrieve this post, this may have been deleted or made private.";
            var viewModel = _contentViewService.CreateNotFoundPage(user, title, description);
            return View(_notFoundRazorFile, viewModel);
        }
        [HttpGet]
        public IActionResult NotFound(NotFoundPage viewModel)
        {
            return View(viewModel);
        }

    }
}
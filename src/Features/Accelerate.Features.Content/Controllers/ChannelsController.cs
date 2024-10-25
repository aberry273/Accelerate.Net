﻿using Accelerate.Features.Content.Models.Views;
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
using System.Threading;
using Twilio.TwiML.Voice;

namespace Accelerate.Features.Content.Controllers
{
    public class ChannelsController : BaseController
    {
        SignInManager<AccountUser> _signInManager;
        UserManager<AccountUser> _userManager;
        IMetaContentService _contentService;
        IContentPostElasticService _contentElasticSearchService;
        IElasticService<ContentPostDocument> _postSearchService;
        IElasticService<ContentChannelDocument> _channelSearchService;
        IEntityService<AccountProfile> _profileService;
        IContentViewService _contentViewService;
        const string _unauthenticatedRedirectUrl = "/Account/login";
        private const string _channelPath = "~/Views/Channel";
        private const string _notFoundRazorFile = "~/Views/Shared/NotFound.cshtml";
        public ChannelsController(
            IMetaContentService service,
            IContentViewService contentViewService,
            IContentPostElasticService postElasticSearchService,
            SignInManager<AccountUser> signInManager,
            IEntityService<ContentPostEntity> postService,
            IEntityService<AccountProfile> profileService,
            IElasticService<ContentPostDocument> searchService,
            IElasticService<ContentChannelDocument> channelService,
            UserManager<AccountUser> userManager) : base(service)
        {
            _signInManager = signInManager;
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
            if (!this.User.Identity.IsAuthenticated)
            {
                return View(_contentViewService.CreateAnonymousChannelsPage());
            }
            var user = await GetUserWithProfile(this.User);
            if (user == null)
            {
                await _signInManager.SignOutAsync();
                return View(_contentViewService.CreateAnonymousChannelsPage());
            }
            //if (user == null) return RedirectToAction("Index", "Account");

            var filters = new List<QueryFilter>()
            {
                _postSearchService.Filter(Foundations.Content.Constants.Fields.PostType, "Post")
            };
            var channels = await _channelSearchService.Search(GetUserChannelsQuery(user), 0, 100);
            var aggResponse = await _postSearchService.GetAggregates(_contentElasticSearchService.CreateThreadAggregateQuery(filters));

            var viewModel = _contentViewService.CreateChannelsPage(user, channels, aggResponse);

            return View(viewModel);
        }

        private async Task<AccountUser> GetUserWithProfile(ClaimsPrincipal principle)
        {
            var user = await _userManager.GetUserAsync(principle);
            if (user == null)
            {
                return null;
            }
            var profile = _profileService.Get(user.AccountProfileId);
            user.AccountProfile = profile;
            return user;
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
        public IActionResult NotFound(NotFoundPage viewModel)
        {
            return View(viewModel);
        }

        [HttpGet]
        [Route("Channels/{id}")]
        [RedirectUnauthenticatedRoute(url = _unauthenticatedRedirectUrl)]
        public async Task<IActionResult> Channel([FromRoute]Guid id)
        {
            return await All(id);
        }
        private string GetChannelView(string routeName)
        {
            return $"{_channelPath}/{routeName}.cshtml";
        }
        [HttpGet]
        [Route("Channels/{id}/All")]
        [RedirectUnauthenticatedRoute(url = _unauthenticatedRedirectUrl)]
        public async Task<IActionResult> All([FromRoute] Guid id)
        {
            var user = await GetUserWithProfile(this.User);
            if (user == null) return RedirectToAction("Index", "Account");

            var response = await _channelSearchService.GetDocument<ContentChannelDocument>(id.ToString());
            var item = response.Source;

            if (item == null)
            {
                return RedirectToAction(nameof(ChannelNotFound));
            }

            var channels = await _channelSearchService.Search(GetUserChannelsQuery(user));
            var aggResponse = await _postSearchService.GetAggregates(_contentElasticSearchService.CreateChannelAggregateQuery(item.Id));

            var viewModel = _contentViewService.CreateChannelPage(user, item, channels, aggResponse);
            viewModel.RouteName = "All";
            return View(this.GetChannelView(nameof(this.All)), viewModel);
        }
        [HttpGet]
        [Route("Channels/{id}/Posts")]
        [RedirectUnauthenticatedRoute(url = _unauthenticatedRedirectUrl)]
        public async Task<IActionResult> Posts([FromRoute] Guid id)
        {
            var user = await GetUserWithProfile(this.User);
            if (user == null) return RedirectToAction("Index", "Account");

            var response = await _channelSearchService.GetDocument<ContentChannelDocument>(id.ToString());
            var item = response.Source;

            if (item == null) return RedirectToAction(nameof(ChannelNotFound));

            var channels = await _channelSearchService.Search(GetUserChannelsQuery(user));
            var aggResponse = await _postSearchService.GetAggregates(_contentElasticSearchService.CreateChannelAggregateQuery(item.Id));

            var viewModel = _contentViewService.CreateChannelPage(user, item, channels, aggResponse);
            viewModel.RouteName = "Posts";
            return View(this.GetChannelView(nameof(this.Posts)), viewModel);
        }

        [HttpGet]
        [Route("Channels/{id}/Related")]
        [RedirectUnauthenticatedRoute(url = _unauthenticatedRedirectUrl)]
        public async Task<IActionResult> Related([FromRoute] Guid id)
        {
            var user = await GetUserWithProfile(this.User);
            if (user == null) return RedirectToAction("Index", "Account");

            var response = await _channelSearchService.GetDocument<ContentChannelDocument>(id.ToString());
            var item = response.Source;

            if (item == null) return RedirectToAction(nameof(ChannelNotFound));

            var channels = await _channelSearchService.Search(GetUserChannelsQuery(user));
            var aggResponse = await _postSearchService.GetAggregates(_contentElasticSearchService.CreateChannelAggregateQuery(item.Id));

            var viewModel = _contentViewService.CreateChannelPage(user, item, channels, aggResponse);
            viewModel.PostsApiUrl = $"{viewModel.PostsApiUrl}/related/{item.Id}";
            viewModel.RouteName = "Related";
            return View(this.GetChannelView(nameof(this.Related)), viewModel);
        }
        [HttpGet]
        [Route("Channels/{id}/Media")]
        [RedirectUnauthenticatedRoute(url = _unauthenticatedRedirectUrl)]
        public async Task<IActionResult> Media([FromRoute] Guid id)
        {
            var user = await GetUserWithProfile(this.User);
            if (user == null) return RedirectToAction("Index", "Account");

            var response = await _channelSearchService.GetDocument<ContentChannelDocument>(id.ToString());
            var item = response.Source;

            if (item == null) return RedirectToAction(nameof(ChannelNotFound));

            var channels = await _channelSearchService.Search(GetUserChannelsQuery(user));
            var aggResponse = await _postSearchService.GetAggregates(_contentElasticSearchService.CreateChannelAggregateQuery(item.Id));

            var viewModel = _contentViewService.CreateChannelPage(user, item, channels, aggResponse);
            viewModel.RouteName = "Media";
            return View(this.GetChannelView(nameof(this.Media)), viewModel);
        }

        [HttpGet]
        [Route("Channels/{id}/Users")]
        [RedirectUnauthenticatedRoute(url = _unauthenticatedRedirectUrl)]
        public async Task<IActionResult> Users([FromRoute] Guid id)
        {
            var user = await GetUserWithProfile(this.User);
            if (user == null) return RedirectToAction("Index", "Account");

            var response = await _channelSearchService.GetDocument<ContentChannelDocument>(id.ToString());
            var item = response.Source;

            if (item == null) return RedirectToAction(nameof(ChannelNotFound));

            var channels = await _channelSearchService.Search(GetUserChannelsQuery(user));
            var aggResponse = await _postSearchService.GetAggregates(_contentElasticSearchService.CreateChannelAggregateQuery(item.Id));

            var viewModel = _contentViewService.CreateChannelPage(user, item, channels, aggResponse);
            viewModel.RouteName = "Users";
            return View(this.GetChannelView(nameof(this.Users)), viewModel);
        }

        private QueryDescriptor<ContentChannelDocument> GetUserChannelsQuery(AccountUser user)
        {
            var query = new QueryDescriptor<ContentChannelDocument>();
            query.MatchAll();
            query.Term(x => x.UserId.Suffix("keyword"), user != null ? user.Id.ToString() : null);
            return query;
        } 
    }
}
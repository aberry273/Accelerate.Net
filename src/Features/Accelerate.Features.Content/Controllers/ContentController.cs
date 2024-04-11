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
using Twilio.TwiML.Voice;

namespace Accelerate.Features.Content.Controllers
{
    public class ContentController : BaseController
    {
        UserManager<AccountUser> _userManager;
        IMetaContentService _contentService;
        IContentPostElasticService _postElasticSearchService;
        IElasticService<ContentPostDocument> _postSearchService;
        IElasticService<ContentChannelDocument> _channelSearchService;
        IContentViewService _contentViewService;
        const string _unauthenticatedRedirectUrl = "/Account/login";
        public ContentController(
            IMetaContentService service,
            IContentViewService contentViewService,
            IContentPostElasticService postElasticSearchService,
            IEntityService<ContentPostEntity> postService,
            IElasticService<ContentPostDocument> searchService,
            IElasticService<ContentChannelDocument> channelService,
            UserManager<AccountUser> userManager) : base(service)
        {
            _userManager = userManager;
            _contentViewService = contentViewService;
            _contentService = service;
            _postSearchService = searchService;
            _postElasticSearchService = postElasticSearchService;
            _channelSearchService = channelService;
        }
        private BasePage CreateBaseContent(AccountUser user)
        {
            var profile = user != null ? new UserProfile()
            {
                Username = user.UserName,
            } : null;
            return _contentService.CreatePageBaseContent(profile);
        }

        [RedirectUnauthenticatedRoute(url = _unauthenticatedRedirectUrl)]
        public async Task<IActionResult> Channels()
        {
            var user = await _userManager.GetUserAsync(this.User);
            if (user == null) return RedirectToAction("Index", "Account");
            var model = CreateBaseContent(user);
            var viewModel = new ChannelsPage(model);
            viewModel.ChannelsDropdown = _contentViewService.GetChannelsDropdown(this.Url.ActionLink("Channels"));

            var channels = await _channelSearchService.Search(GetUserChannelsQuery(user));
            if(channels != null && channels.IsValidResponse)
            {
                var channelItems = channels.Documents.Select(x => new NavigationItem()
                {
                    Text = x.Name,
                    Href = "/Content/Channel/" + x.Id
                });
                viewModel.ChannelsDropdown.Items.AddRange(channelItems);
            }

            viewModel.UserId = user.Id;
            viewModel.FormCreateReply = _contentViewService.CreatePostForm(user);
            viewModel.ModalCreateChannel = _contentViewService.CreateModalChannelForm(user);
            viewModel.ModalEditReply = _contentViewService.CreateModalEditReplyForm(user);
            viewModel.ModalDeleteReply = _contentViewService.CreateModalDeleteReplyForm(user);
            return View(viewModel);
        }

        [RedirectUnauthenticatedRoute(url = _unauthenticatedRedirectUrl)]
        public async Task<IActionResult> Channel([FromRoute] Guid id)
        {
            var user = await _userManager.GetUserAsync(this.User);
            if (user == null) return RedirectToAction("Index", "Account");
            var model = CreateBaseContent(user);

            var viewModel = new ChannelPage(model);
            var response = await _channelSearchService.GetDocument<ContentChannelDocument>(id.ToString());
            var item = response.Source;
            
            if (item == null) return RedirectToAction(nameof(ChannelNotFound)); 
            else viewModel.Item = item; 
            
            var channelsResponse = await _channelSearchService.Search(GetUserChannelsQuery(user));
            viewModel.ChannelsDropdown = _contentViewService.GetChannelsDropdown(this.Url.ActionLink("Channels"), channelsResponse, item.Name);
            
            // Add filters
            var filters = new List<QueryFilter>()
            {
                _postSearchService.Filter(Foundations.Content.Constants.Fields.TargetChannel, ElasticCondition.Filter, item.Id)
            };
            var aggregates = new List<string>()
            {
                Foundations.Content.Constants.Fields.TargetThread.ToCamelCase(),
                Foundations.Content.Constants.Fields.Tags.ToCamelCase(),
            };
            var requestFilters = new RequestQuery<ContentPostDocument>() { Filters = filters, Aggregates = aggregates };
             
            var aggResponse = await _postSearchService.GetAggregates(requestFilters);
            viewModel.Filters = _contentViewService.CreateSearchFilters(aggResponse);

            viewModel.UserId = user.Id;
            viewModel.FormCreateReply = _contentViewService.CreatePostForm(user, item);
            viewModel.ModalCreateChannel = _contentViewService.CreateModalChannelForm(user);
            viewModel.ModalEditReply = _contentViewService.CreateModalEditReplyForm(user);
            viewModel.ModalDeleteReply = _contentViewService.CreateModalDeleteReplyForm(user);
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
            var user = await _userManager.GetUserAsync(this.User);
            if (user == null) return RedirectToAction("Index", "Account");
            var model = CreateBaseContent(user);
            var viewModel = new ThreadPage(model);
            var response = await _postSearchService.GetDocument<ContentPostDocument>(id.ToString());
            
            var item = response.Source;

            if (item == null) return RedirectToAction(nameof(PostNotFound));
            else viewModel.Item = item;

            viewModel.UserId = user.Id;
            viewModel.PreviousUrl = Request.Headers["Referer"].ToString();
            // Get replies


            var query = this._postElasticSearchService.BuildRepliesSearchQuery(item.Id.ToString());
            var replies = await _postSearchService.Search(query, 0, 100);

            //var replies = await _postSearchService.Search(GetRepliesQuery(item), 0, 1000);
            viewModel.Replies = replies.Documents.ToList();
            viewModel.FormCreateReply = _contentViewService.CreateReplyForm(user, item);
            viewModel.ModalEditReply = _contentViewService.CreateModalEditReplyForm(user);
            viewModel.ModalDeleteReply = _contentViewService.CreateModalDeleteReplyForm(user);

            // Add filters
            var filters = new List<QueryFilter>()
            {
                _postSearchService.Filter(Foundations.Content.Constants.Fields.ParentId, item.Id)
            };

            var aggregates = new List<string>()
            {
                Foundations.Content.Constants.Fields.TargetThread.ToCamelCase(),
                Foundations.Content.Constants.Fields.Tags.ToCamelCase(),
            };
            var requestFilters = new RequestQuery<ContentPostDocument>() { Filters = filters, Aggregates = aggregates };

            var aggResponse = await _postSearchService.GetAggregates(requestFilters);
            viewModel.Filters = _contentViewService.CreateSearchFilters(aggResponse);

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
            var user = await _userManager.GetUserAsync(this.User);
            if (user == null) return RedirectToAction("Index", "Account");
            var model = CreateBaseContent(user);
            var viewModel = new NotFoundPage(model);
            viewModel.Title = "Channel not found";
            viewModel.Description = "We are unable to retrieve this channel, this may have been deleted or made private.";
            return NotFound(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> PostNotFound()
        {
            var user = await _userManager.GetUserAsync(this.User);
            if (user == null) return RedirectToAction("Index", "Account");
            var model = CreateBaseContent(user);
            var viewModel = new NotFoundPage(model);
            viewModel.Title = "Post not found";
            viewModel.Description = "We are unable to retrieve this post, this may have been deleted or made private.";
            return NotFound(viewModel);
        }
        [HttpGet]
        public IActionResult NotFound(NotFoundPage viewModel)
        {
            return View("~/Views/Content/NotFound.cshtml", viewModel);
        }

    }
}
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
using System.Threading;
using Twilio.TwiML.Voice;

namespace Accelerate.Features.Content.Controllers
{
    public class ContentController : BaseController
    {
        SignInManager<AccountUser> _signInManager;
        UserManager<AccountUser> _userManager;
        IMetaContentService _contentService;
        IContentPostElasticService _contentElasticSearchService;
        IElasticService<ContentPostDocument> _postSearchService;
        IElasticService<ContentChannelDocument> _channelSearchService;
        IElasticService<ContentFeedDocument> _feedSearchService;
        IElasticService<ContentListDocument> _listSearchService;
        IElasticService<ContentChatDocument> _chatSearchService;
        IEntityService<AccountProfile> _profileService;
        IContentViewService _contentViewService;
        const string _unauthenticatedRedirectUrl = "/Account/login";
        private const string _channelPath = "~/Views/Channel";
        private const string _notFoundRazorFile = "~/Views/Shared/NotFound.cshtml";
        public ContentController(
            IMetaContentService service,
            IContentViewService contentViewService,
            IContentPostElasticService postElasticSearchService,
            SignInManager<AccountUser> signInManager,
            IEntityService<ContentPostEntity> postService,
            IEntityService<AccountProfile> profileService,
            IElasticService<ContentPostDocument> searchService,
            IElasticService<ContentChannelDocument> channelService,
            IElasticService<ContentFeedDocument> feedService,
            IElasticService<ContentListDocument> listService,
            IElasticService<ContentChatDocument> chatSearchService,
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
            _feedSearchService = feedService;
            _listSearchService = listService;
            _chatSearchService = chatSearchService;
        }
        private const string _contentCreatePageRazor = "~/Views/Shared/ContentCreatePage.cshtml";
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
        [Route("Content/Channel/Edit/{id}")]
        [HttpGet]
        public async Task<IActionResult> EditChannel([FromRoute] Guid id)
        {
            var user = await GetUserWithProfile(this.User);
            var channels = await _channelSearchService.Search(GetUserChannelsQuery(user));
            var item = await _channelSearchService.GetDocument<ContentChannelDocument>(id.ToString());
            var viewModel = _contentViewService.CreateChannelEditPage(user, channels, item.Source);
            return View(_contentCreatePageRazor, viewModel);
        }
        [Route("Content/Channel/Create")]
        [HttpGet]
        public async Task<IActionResult> CreateChannel()
        {
            var user = await GetUserWithProfile(this.User);
            var channels = await _channelSearchService.Search(GetUserChannelsQuery(user));
            
            var viewModel = _contentViewService.CreateChannelCreatePage(user, channels);
            return View(_contentCreatePageRazor, viewModel);
        }
        private QueryDescriptor<ContentChannelDocument> GetUserChannelsQuery(AccountUser user)
        {
            var query = new QueryDescriptor<ContentChannelDocument>();
            query.MatchAll();
            query.Term(x => x.UserId.Suffix("keyword"), user != null ? user.Id.ToString() : null);
            return query;
        }


        [Route("Content/List/Edit/{id}")]
        [HttpGet]
        public async Task<IActionResult> EditList([FromRoute] Guid id)
        {
            var user = await GetUserWithProfile(this.User);
            var lists = await _listSearchService.Search(GetUserListsQuery(user));

            var item = await _listSearchService.GetDocument<ContentListDocument>(id.ToString());
            var viewModel = _contentViewService.CreateListEditPage(user, lists, item.Source);
            return View(_contentCreatePageRazor, viewModel);
        }
        [Route("Content/List/Create")]
        [HttpGet]
        public async Task<IActionResult> CreateList()
        {
            var user = await GetUserWithProfile(this.User);
            var channels = await _listSearchService.Search(GetUserListsQuery(user));

            var viewModel = _contentViewService.CreateListCreatePage(user, channels);
            return View(_contentCreatePageRazor, viewModel);
        }
        private QueryDescriptor<ContentListDocument> GetUserListsQuery(AccountUser user)
        {
            var query = new QueryDescriptor<ContentListDocument>();
            query.MatchAll();
            query.Term(x => x.UserId.Suffix("keyword"), user != null ? user.Id.ToString() : null);
            return query;
        }
        [Route("Content/Chat/Edit/{id}")]
        [HttpGet]
        public async Task<IActionResult> EditChat([FromRoute] Guid id)
        {
            var user = await GetUserWithProfile(this.User);
            var chats = await _feedSearchService.Search(GetUserChatsQuery(user));

            var item = await _chatSearchService.GetDocument<ContentChatDocument>(id.ToString());
            var viewModel = _contentViewService.CreateChatEditPage(user, chats, item.Source);
            return View(_contentCreatePageRazor, viewModel);
        }
        [Route("Content/Chat/Create")]
        [HttpGet]
        public async Task<IActionResult> CreateChat()
        {
            var user = await GetUserWithProfile(this.User);
            var chats = await _postSearchService.Search(GetUserChatsQuery(user));

            var viewModel = _contentViewService.CreateChatCreatePage(user, chats);
            return View(_contentCreatePageRazor, viewModel);
        }
        private QueryDescriptor<ContentChatDocument> GetUserChatsQuery(AccountUser user)
        {
            var query = new QueryDescriptor<ContentChatDocument>();
            query.MatchAll();
            query.Term(x => x.UserId.Suffix("keyword"), user != null ? user.Id.ToString() : null);
            return query;
        }
        [Route("Content/Feed/Create")]
        [HttpGet]
        public async Task<IActionResult> CreateFeed()
        {
            var user = await GetUserWithProfile(this.User);
            var feeds = await _feedSearchService.Search(GetUserFeedsQuery(user));

            var viewModel = _contentViewService.CreateFeedCreatePage(user, feeds);
            return View(_contentCreatePageRazor, viewModel);
        }
        [Route("Content/Feed/Edit/{id}")]
        [HttpGet]
        public async Task<IActionResult> EditFeed([FromRoute] Guid id)
        {
            var user = await GetUserWithProfile(this.User);
            var feeds = await _feedSearchService.Search(GetUserFeedsQuery(user));

            var item = await _feedSearchService.GetDocument<ContentFeedDocument>(id.ToString());
            var viewModel = _contentViewService.CreateFeedEditPage(user, feeds, item.Source);
            return View(_contentCreatePageRazor, viewModel);
        }
        private QueryDescriptor<ContentFeedDocument> GetUserFeedsQuery(AccountUser user)
        {
            var query = new QueryDescriptor<ContentFeedDocument>();
            query.MatchAll();
            query.Term(x => x.UserId.Suffix("keyword"), user != null ? user.Id.ToString() : null);
            return query;
        }
    }
}
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
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Aggregations;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Features.Content.Controllers
{
    public class ContentController : BaseController
    {
        UserManager<AccountUser> _userManager;
        IMetaContentService _contentService;
        IElasticService<ContentPostDocument> _searchService;
        IContentViewService _contentViewService;
        const string _unauthenticatedRedirectUrl = "/Account/login";
        public ContentController(
            IMetaContentService service,
            IContentViewService contentViewService,
            IEntityService<ContentPostEntity> postService,
            IElasticService<ContentPostDocument> searchService,
            UserManager<AccountUser> userManager) : base(service)
        {
            _userManager = userManager;
            _contentViewService = contentViewService;
            _contentService = service;
            _searchService = searchService;
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
        public async Task<IActionResult> Feed()
        {
            var user = await _userManager.GetUserAsync(this.User);
            if(user == null)
            {
                return RedirectToAction("Index", "Account");
            }
            var model = CreateBaseContent(user);
            var viewModel = new FeedPage(model);
            viewModel.UserId = user.Id;
            viewModel.FormCreateReply = _contentViewService.CreatePostForm(user);
            viewModel.ModalEditReply = _contentViewService.CreateModalEditReplyForm(user);
            viewModel.ModalDeleteReply = _contentViewService.CreateModalDeleteReplyForm(user);
            return View(viewModel);
        }

        [HttpGet]
       // [Route("{id}")]
        [RedirectUnauthenticatedRoute(url = _unauthenticatedRedirectUrl)]
        public async Task<IActionResult> Thread([FromRoute] Guid id)
        {
            var user = await _userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return RedirectToAction("Index", "Account");
            }
            var model = CreateBaseContent(user);
            var viewModel = new ThreadPage(model);
            var response = await _searchService.GetDocument<ContentPostDocument>(id.ToString());
            viewModel.UserId = user.Id;
            viewModel.PreviousUrl = Request.Headers["Referer"].ToString();
            var item = response.Source;
            if(item == null)
            {
                viewModel.Item = new ContentPostDocument()
                {
                    Content = "TODO: REPLACE WITH 404 PAGE"
                };
            }
            else
            {
                viewModel.Item = item;
            }
            // Get replies
            var replies = await _searchService.Search(GetRepliesQuery(item), 0, 1000);
            viewModel.Replies = replies.Documents.ToList();
            viewModel.FormCreateReply = _contentViewService.CreateReplyForm(user, item);
            viewModel.ModalEditReply = _contentViewService.CreateModalEditReplyForm(user);
            viewModel.ModalDeleteReply = _contentViewService.CreateModalDeleteReplyForm(user);

            var aggQuery = new RequestQuery<ContentPostDocument>();
            var aggResponse = await _searchService.GetAggregates(aggQuery);
            var tags = new List<string>();
            var threads = new List<string>();
            if (aggResponse.IsValidResponse)
            {
                //tags
                tags = GetValuesFromAggregate(aggResponse.Aggregations, "tags");
                //threads
                threads = GetValuesFromAggregate(aggResponse.Aggregations, "threadIds");
            }

            viewModel.Filters = CreateNavigationFilters(tags, threads);

            return View(viewModel);
        }

        private List<string> GetValuesFromAggregate(AggregateDictionary aggregates, string key)
        {
            var agg = aggregates.FirstOrDefault(x => x.Key == key);
            StringTermsAggregate vals = agg.Value as StringTermsAggregate;
            var results = vals.Buckets.Select(x => x.Key.Value.ToString()).ToList();
            return results;
        }

        private List<NavigationFilter> CreateNavigationFilters(List<string> tags, List<string> threads)
        {
            return new List<NavigationFilter>()
            {
                new NavigationFilter() {
                    Name = "Reviews",
                    FilterType = NavigationFilterType.Select,
                    Values = new List<string>
                    {
                        "All", "Agrees", "Disagrees"
                    }
                },
                new NavigationFilter() {
                    Name = "Threads",
                    FilterType = NavigationFilterType.Checkbox,
                    Values = threads
                },
                new NavigationFilter() {
                    Name = "Tags",
                    FilterType = NavigationFilterType.Checkbox,
                    Values = tags
                },
                new NavigationFilter() {
                    Name = "Content",
                    FilterType = NavigationFilterType.Checkbox,
                    Values = new List<string>
                    {
                        "All", "Agrees", "Disagrees"
                    }
                },
                new NavigationFilter() {
                    Name = "Sort",
                    FilterType = NavigationFilterType.Radio,
                    Values = new List<string>
                    {
                        "All", "Agrees", "Disagrees"
                    }
                }
            };
        }
        private QueryDescriptor<ContentPostDocument> GetRepliesQuery(ContentPostDocument item)
        {
            var query = new QueryDescriptor<ContentPostDocument>();
            query.MatchAll();
            query.Term(x => x.TargetThread.Suffix("keyword"), item.ThreadId.ToString());
            return query;
        }

    }
}
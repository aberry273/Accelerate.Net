using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Content.Models.Data;
using Elastic.Clients.Elasticsearch.Aggregations;
using Elastic.Clients.Elasticsearch;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Content.Services;
using Microsoft.AspNetCore.Mvc;
using Accelerate.Foundations.Content.Models.View;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Account.Services;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Microsoft.VisualBasic;
using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Features.Content.Hydrators;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Accelerate.Features.Content.Controllers;
using Accelerate.Foundations.Common.Helpers;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Hydrators;
using static MassTransit.ValidationResultExtensions;
using Accelerate.Foundations.Content.EventBus;
using Accelerate.Foundations.EventPipelines.Services;

namespace Accelerate.Features.Content.Services
{
    public class ContentViewSearchService : IContentViewSearchService
    {
        IContentPostElasticService _searchService;
        IEntityService<ContentPostPinEntity> _pinnedContentService;
        IEntityService<ContentPostParentEntity> _servicePostParents { get; set; }

        IAccountUserSearchService _userSearchService;
        IElasticService<ContentChannelDocument> _searchChannelService;
        IElasticService<AccountUserDocument> _accountElasticService;
        public ContentViewSearchService(
            IContentPostElasticService searchService,
            IEntityService<ContentPostPinEntity> pinnedContentService,
            IEntityService<ContentPostParentEntity> servicePostParents,
            IAccountUserSearchService userSearchService,
            IElasticService<ContentChannelDocument> searchChannelService,
            IElasticService<AccountUserDocument> accountElasticService)
        {
            _searchService = searchService;
            _pinnedContentService = pinnedContentService;
            _userSearchService = userSearchService;
            _searchChannelService = searchChannelService;
            _accountElasticService = accountElasticService;
            _servicePostParents = servicePostParents;
        }
        #region Search methods
        public async Task<ContentSearchResults> SearchPosts(RequestQuery query, Guid postId)
        {
            var sortOrder = this.GetSortOrderField(query.Filters);
            query.Filters = this.GetActualFilterKeys(query.Filters);
            var result = await _searchService.SearchPost(query, postId, GetFilterSortField(query), sortOrder);
            result.Posts = await this.UpdatePostDocuments(result.Posts.ToList());
            return result;
        }
        public async Task<ContentSearchResults> SearchUserPosts(RequestQuery query, Guid userId)
        {
            query.Filters = this.GetActualFilterKeys(query.Filters);
            var result = await _searchService.SearchUserPosts(userId, query.Page, query.ItemsPerPage);
            result.Posts = await this.UpdatePostDocuments(result.Posts.ToList());
            return result;
        }

        public async Task<ContentSearchResults> SearchPostParents(Guid postId, RequestQuery query)
        {
            query.Filters = this.GetActualFilterKeys(query.Filters);
            var result = await _searchService.SearchPostParents(query, postId, query.UserId.GetValueOrDefault());
            result.Posts = await this.UpdatePostDocuments(result.Posts.ToList());
            return result;
        }
        public async Task<List<ContentPostPinDocument>> SearchPostPinned(Guid postId, RequestQuery query)
        {
            var pinned = _pinnedContentService.Find(x => x.ContentPostId == postId);
            var pinnedIds = pinned.Select(x => x.PinnedContentPostId.ToString()).ToList();
            query.ItemsPerPage = 10;
            query.Page = 0;
            var posts = await _searchService.SearchPostByIds(query, pinnedIds);
            var userIds = pinned.Select(x => x.UserId.ToString()).ToList();
            var users = await _userSearchService.SearchUsers(query, userIds);

            var pinnedDocuments = pinned.Select(x =>
            {
                var post = posts.FirstOrDefault(y => y.Id == x.PinnedContentPostId);
                var user = users.Users.FirstOrDefault(y => y.Id == x.UserId);
                return new ContentPostPinDocument()
                {
                    ContentPost = post,
                    Date = Foundations.Common.Extensions.DateExtensions.ToDateSimple(x.CreatedOn),
                    Id = x.Id,
                    ContentPostId = x.ContentPostId,
                    Reason = x.Reason,
                    Href = GetPostHref(post),
                    UserId = post?.UserId,
                    Username = user?.Username,
                };
            }).ToList();

            return pinnedDocuments;
        }
        public async Task<ContentSearchResults> SearchPostRepliesFromRoute(Guid postId, RequestQuery query)
        {
            SortOrder sortOrder = SortOrder.Desc;
            Enum.TryParse<SortOrder>(query.SortBy, out sortOrder);
            query.Filters = this.GetActualFilterKeys(query.Filters);
            var result = await _searchService.SearchPostReplies(postId, query, GetFilterSortField(query), sortOrder);
            result.Posts = await this.UpdatePostDocuments(result.Posts.ToList());
            return result;
        }
        public async Task<ContentSearchResults> SearchPostReplies(RequestQuery query)
        {
            SortOrder sortOrder = SortOrder.Desc;
            Enum.TryParse<SortOrder>(query.SortBy, out sortOrder);
            query.Filters = this.GetActualFilterKeys(query.Filters);
            var result = await _searchService.SearchPostReplies(query, GetFilterSortField(query), sortOrder);
            var posts = result.Posts.ToList();
            result.Posts = await this.UpdatePostDocuments(posts);
            return result;
        }
        public async Task<ContentSearchResults> SearchPosts(RequestQuery query)
        {
            SortOrder sortOrder = SortOrder.Desc;
            Enum.TryParse<SortOrder>(query.SortBy, out sortOrder);
            query.Filters = this.GetActualFilterKeys(query.Filters);
            var sortBy = this.GetFilterSortOptions().FirstOrDefault(x => x.Name == query.Sort);
            var result = await _searchService.SearchPosts(query, GetFilterSortField(query), sortOrder);
            result.Posts = await this.UpdatePostDocuments(result.Posts.ToList());
            return result;
        }
        public async Task<ContentSearchResults> SearchPostsRelated(Guid channelId, RequestQuery query)
        {
            query.Filters = this.GetActualFilterKeys(query.Filters);
            var channel = await _searchChannelService.GetDocument<ContentChannelDocument>(channelId.ToString());
            var result = await _searchService.SearchRelatedPosts(channel.Source, query);
            result.Posts = await this.UpdatePostDocuments(result.Posts.ToList());
            return result;
        }
        public async Task<ContentSearchResults> SearchChannels(RequestQuery query)
        {
            query.Filters = this.GetActualFilterKeys(query.Filters);
            var result = await _searchService.SearchPosts(query);
            result.Posts = await this.UpdatePostDocuments(result.Posts.ToList());
            return result;
        }
        #endregion

        #region Helper methods 
        private async Task<List<AccountUserDocument>> GetUsers(IEnumerable<Guid> UserIds)
        {
            var userIds = UserIds.Distinct().Select(x => x.ToString()).ToList();
            var query = new RequestQuery() { ItemsPerPage = userIds.Count() };
            var userResult = await _userSearchService.SearchUsers(query, userIds);
            return userResult.Users;
        }
        private async Task<List<AccountUserDocument>> GetUserProfiles(IEnumerable<Guid> userIds)
        {
            var userIdStrings = userIds.Select(x => x.ToString()).ToList();
            var query = new RequestQuery() { ItemsPerPage = userIds.Count() };
            var userResult = await _userSearchService.SearchUsers(query, userIdStrings);
            return userResult.Users;
        }
        private IEnumerable<ContentPostParentEntity> GetReplies(IEnumerable<ContentPostDocument> posts)
        {
            var postIds = posts
                .Select(x => x?.Id)
                .Distinct()
                .ToList();
            return _servicePostParents.Find(x => postIds.Contains(x.ParentId));
        }
        public async Task<List<ContentPostViewDocument>> UpdatePostDocuments(List<ContentPostViewDocument> posts)
        {
            // Get list of userIds for each post
            List<Guid> userIds = posts.Select(x => x.UserId).ToList();
            // Get all Post/Parent entities where the post is a parent
            var postReplies = GetReplies(posts);
            var allPostReplyUserIds = postReplies.Select(x => x.UserId);
            // Add Post Reply userIds to list
            userIds.AddRange(allPostReplyUserIds);
            // Get user entity for each ID
            var users = await GetUsers(userIds);
            //Create user profile objects for each user
            var userProfiles = users.Select(GetUserDocument);
            for (var i = 0; i < posts.Count(); i++)
            {
                // Get the users for all parentPosts
                var postReplyUserIds = postReplies
                    .Where(x => x.ParentId == posts[i].Id)
                    .Select(x => x.UserId);
                var replyProfiles = userProfiles
                    .Where(x => postReplyUserIds.Contains(x.Id))
                    .ToList();
                // Users
                var postUserProfile = userProfiles?.FirstOrDefault(x => x.Id == posts[i].UserId);
                var postReplyProfiles = replyProfiles?.Where(x => x.Id == posts[i].UserId).ToList();
                posts[i] = this.UpdateViewPostModel(posts[i], postUserProfile, postReplyProfiles);
            }
            return posts.ToList();
        }
        public async Task<ContentPostViewDocument> UpdatePostDocument(ContentPostDocument post)
        {
            // Get all Post/Parent entities where the post is a parent
            var replies = this.GetReplies(new List<ContentPostDocument>() { post });
            // Add Post Reply userIds to list
            var postReplyUserIds = replies.Select(x => x.UserId).ToList();
            // Add current post user to list
            var userIds = new List<Guid>(postReplyUserIds) { post.UserId };
            // Get user entity for each ID
            var users = await GetUsers(userIds);
            //Create user profile objects for each user
            var userProfiles = users.Select(GetUserDocument);
            var replyProfiles = userProfiles.Where(x => postReplyUserIds.Contains(x.Id));
            // Users
            var postUserProfile = userProfiles?.FirstOrDefault(x => x.Id == post.UserId);
            var postReplyProfiles = replyProfiles?.Where(x => x.Id == post.UserId).ToList();
            var viewModel = this.UpdateViewPostModel(post, postUserProfile, postReplyProfiles);
            return viewModel;
        }

        public ContentPostViewDocument UpdateViewPostModel(ContentPostDocument post, ContentPostUserProfileSubdocument profile, List<ContentPostUserProfileSubdocument> Replies = null)
        {
            var viewModel = new ContentPostViewDocument();
            viewModel.HydrateFrom(post);
            // Users
            viewModel.Profile = profile;
            // Href
            viewModel.Ui.Href = GetPostHref(post);
            // Replies
            if(Replies != null)
            {
                viewModel.Replies = new ContentPostRepliesSubdocument()
                {
                    Profiles = Replies,
                    Text = "Replies",
                    Date = "01/01/2000"
                };
            }
            // Metrics
            viewModel.Metrics = new ContentPostMetricsSubdocument()
            {
                Replies = 22,
                Quotes = 1,
                Rating = 10
            };
            viewModel.Actions = this.PostActions();
            viewModel.Menu = this.PostMenuActions();
            return viewModel;
        }
        private List<string> PostActions()
        {
            return new List<string>()
            {
                "reply", "open", "quote", "upvote", "downvote", "tag"
            };
        }
        private List<string> PostMenuActions()
        {
            return new List<string>()
            {
                "CopyLink", "Flag", "Delete", "Edit"
            };
        }

        private ContentPostUserProfileSubdocument GetUserDocument(AccountUserDocument user)
        {
            var model = new ContentPostUserProfileSubdocument();
            user?.Hydrate(model);
            return model;
        }

        private string GetPostHref(ContentPostDocument post)
        {
            return $"/{ControllerHelper.NameOf<ThreadsController>()}/{post.Id}";
        }

        private string GetFilterSortField(RequestQuery query)
        {
            var sortBy = this.GetFilterSortOptions().FirstOrDefault(x => x.Name == query.Sort);
            return sortBy?.Key ?? this.GetFilterSortOptions().FirstOrDefault().Key;
        }
        public List<QueryFilter>? GetActualFilterKeys(List<QueryFilter>? Filters)
        {
            return Filters
                ?
                .Select(x =>
                {
                    x.Name = GetFilterKey(x.Name);
                    return x;
                }).ToList();
        }
        public List<KeyValuePair<string, string>> GetFilterOptions()
        {
            return new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(Constants.Filters.Category, Foundations.Content.Constants.Fields.Category.ToCamelCase()),
                new KeyValuePair<string, string>(Constants.Filters.Labels, Foundations.Content.Constants.Fields.Labels.ToCamelCase()),
                new KeyValuePair<string, string>(Constants.Filters.Tags, Foundations.Content.Constants.Fields.Tags.ToCamelCase()),
                new KeyValuePair<string, string>(Constants.Filters.Votes, Foundations.Content.Constants.Fields.ParentVote.ToCamelCase()),
                new KeyValuePair<string, string>(Constants.Filters.Threads, Foundations.Content.Constants.Fields.ShortThreadId.ToCamelCase()),
                new KeyValuePair<string, string>(Constants.Filters.Quotes, Foundations.Content.Constants.Fields.QuoteIds.ToCamelCase()),
                new KeyValuePair<string, string>(Constants.Filters.Sort, Constants.Filters.Sort),
            };
        }

        private string GetFilterOptionKey(string filterValue)
        {
            return GetFilterOptions().FirstOrDefault(y => y.Value == filterValue).Value;
        }

        public string GetFilterKey(string key)
        {
            var keyVal = this.GetFilterOptions().FirstOrDefault(x => x.Key == key);
            if (keyVal.Value == null) return key.ToCamelCase();
            return keyVal.Value?.ToCamelCase();
        }
        private List<NavigationFilterValue> GetAggregateValues(IDictionary<string, List<NavigationFilterValue>> aggFilters, string key)
        {
            if (key == null) return new List<NavigationFilterValue>();
            return aggFilters.ContainsKey(key) ? aggFilters[key] : new List<NavigationFilterValue>();
        }
        private List<NavigationFilterItem> CreateNavigationFilters(IDictionary<string, List<NavigationFilterValue>> filters)
        {
            if (filters == null) filters = new Dictionary<string, List<NavigationFilterValue>>();
            var filter = new List<NavigationFilterItem>();

            var Votes = GetAggregateValues(filters, GetFilterKey(Constants.Filters.Votes));
            if (Votes.Count > 0)
            {
                filter.Add(new NavigationFilterItem()
                {
                    Name = Constants.Filters.Votes,
                    FilterType = NavigationFilterType.Radio,
                    Values = Votes
                });
            }
            var Actions = GetAggregateValues(filters, GetFilterKey(Constants.Filters.Actions));
            if (Actions.Count > 0)
            {
                filter.Add(new NavigationFilterItem()
                {
                    Name = Constants.Filters.Actions,
                    FilterType = NavigationFilterType.Select,
                    Values = Actions
                });
            }
            var threads = GetAggregateValues(filters, GetFilterKey(Constants.Filters.Threads));
            if (threads.Count > 0)
            {
                filter.Add(new NavigationFilterItem()
                {
                    Name = Constants.Filters.Threads,
                    FilterType = NavigationFilterType.Checkbox,
                    Values = threads
                });
            }
            var quotes = GetAggregateValues(filters, GetFilterKey(Constants.Filters.Quotes));
            if (quotes.Count > 0)
            {
                filter.Add(new NavigationFilterItem()
                {
                    Name = Constants.Filters.Quotes,
                    FilterType = NavigationFilterType.Checkbox,
                    Values = quotes
                });
            }

            var tags = GetAggregateValues(filters, GetFilterKey(Constants.Filters.Tags));
            if (tags.Count > 0)
            {
                filter.Add(new NavigationFilterItem()
                {
                    Name = Constants.Filters.Tags,
                    FilterType = NavigationFilterType.Checkbox,
                    Values = tags
                });
            }


            var labels = GetAggregateValues(filters, GetFilterKey(Constants.Filters.Labels));
            if (labels.Count > 0)
            {
                filter.Add(new NavigationFilterItem()
                {
                    Name = Constants.Filters.Labels,
                    FilterType = NavigationFilterType.Checkbox,
                    Values = labels
                });
            }

            var content = GetAggregateValues(filters, GetFilterKey(Constants.Filters.Content));
            if (content.Count > 0)
            {
                filter.Add(new NavigationFilterItem()
                {
                    Name = Constants.Filters.Content,
                    FilterType = NavigationFilterType.Select,
                    Values = content
                });
            }

            return filter;
        }
        public List<NavigationFilterValue> GetFilterSortOptions()
        {
            return new List<NavigationFilterValue>()
            {
                new NavigationFilterValue()
                {
                    Key = Foundations.Content.Constants.Fields.CreatedOn,
                    Name = "Created"
                },
                new NavigationFilterValue()
                {
                    Key = Foundations.Content.Constants.Fields.UpdatedOn,
                    Name = "Updated"
                },
                new NavigationFilterValue()
                {
                    Key = Foundations.Content.Constants.Fields.Replies,
                    Name = "Replies"
                },
                new NavigationFilterValue()
                {
                    Key = Foundations.Content.Constants.Fields.Quotes,
                    Name = "Quotes"
                },
                new NavigationFilterValue()
                {
                    Key = Foundations.Content.Constants.Fields.TotalVotes,
                    Name = "Total Votes"
                },
            };
        }
        public List<NavigationFilterValue> GetFilterSortOrderOptions()
        {
            return new List<NavigationFilterValue>()
            {
                new NavigationFilterValue()
                {
                    Key = "Asc",
                    Name = "Asc"
                },
                new NavigationFilterValue()
                {
                    Key = "Desc",
                    Name = "Desc"
                },
            };
        }

        public Elastic.Clients.Elasticsearch.SortOrder GetSortOrderField(List<QueryFilter>? Filters)
        {
            var sortField = Filters.FirstOrDefault(x => x.Name == Constants.Filters.SortOrder);
            if (sortField == null)
            {
                return Elastic.Clients.Elasticsearch.SortOrder.Desc;
            }
            var val = sortField.Value?.ToString();
            if (string.IsNullOrEmpty(val)) return Elastic.Clients.Elasticsearch.SortOrder.Desc;
            if (val == "Asc") return Elastic.Clients.Elasticsearch.SortOrder.Asc;
            return Elastic.Clients.Elasticsearch.SortOrder.Desc;
        }
        public string? GetSortField(List<QueryFilter>? Filters)
        {
            var sortField = Filters.FirstOrDefault(x => x.Name == Constants.Filters.Sort);
            if (sortField == null)
            {
                return null;
            }
            var val = sortField.Value?.ToString();
            if (string.IsNullOrEmpty(val)) return null;
            var option = GetFilterSortOptions().FirstOrDefault(x => x.Key == val);
            return option.Key;
        }
        public NavigationFilter CreateNavigationFilters(SearchResponse<ContentPostDocument> aggregateResponse)
        {
            return new NavigationFilter()
            {
                Filters = CreateSearchFilters(aggregateResponse),
                Sort = new NavigationFilterItem()
                {
                    Name = Constants.Filters.Sort,
                    FilterType = NavigationFilterType.Select,
                    Values = GetFilterSortOptions()
                },
                SortBy = new NavigationFilterItem()
                {
                    Name = Constants.Filters.SortOrder,
                    FilterType = NavigationFilterType.Select,
                    Values = GetFilterSortOrderOptions()
                }
            };
        }
        public List<NavigationFilterItem> CreateSearchFilters(SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var filterValues = new Dictionary<string, List<NavigationFilterValue>>();
            if (aggregateResponse.IsValidResponse)
            {
                var filterOptions = GetFilterOptions();
                filterValues = filterOptions
                    .Select(x => x.Value)
                    .ToDictionary(x => x, x => GetValuesFromAggregate(aggregateResponse.Aggregations, x));
            }
            return CreateNavigationFilters(filterValues);
        }
        private List<NavigationFilterValue> GetValuesFromAggregate(AggregateDictionary aggregates, string key)
        {
            var agg = aggregates.FirstOrDefault(x => x.Key == key);
            StringTermsAggregate vals = agg.Value as StringTermsAggregate;
            if (vals == null || vals.Buckets == null || vals.Buckets.Count == 0) return new List<NavigationFilterValue>();

            var results = vals.Buckets
                .Where(x => !string.IsNullOrEmpty(x.Key.Value.ToString()))
                .Select(x => new NavigationFilterValue()
                {

                    Key = x.Key.Value.ToString(),
                    Name = x.Key.Value.ToString(),
                    Count = x.DocCount
                }).
                ToList();
            return results;
        }
        #endregion
    }
}

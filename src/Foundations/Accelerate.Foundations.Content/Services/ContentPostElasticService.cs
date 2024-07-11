using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;
using Microsoft.Extensions.Options;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Elastic.Clients.Elasticsearch.Mapping;
using Elastic.Clients.Elasticsearch.Aggregations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Accelerate.Foundations.Common.Extensions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Elastic.Clients.Elasticsearch.Core.TermVectors; 
using Accelerate.Foundations.Content.Models.Entities;
using static Elastic.Clients.Elasticsearch.JoinField;
using System.Threading;
using Azure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Accelerate.Foundations.Content.Services
{
    //Overwrite the core service for custom filtering
    public class ContentPostElasticService : ElasticService<ContentPostDocument>, IContentPostElasticService
    {
        IElasticService<ContentPostActionsDocument> _actionSearchService;
        IElasticService<ContentPostActionsSummaryDocument> _actionSummarySearchService;
        public ContentPostElasticService(
            IOptions<ElasticConfiguration> options,
            IOptions<ContentConfiguration> config,
            IElasticService<ContentPostActionsDocument> actionSearchService,
            IElasticService<ContentPostActionsSummaryDocument> actionSummarySearchService
            ) : base(options)
        {
            _actionSearchService = actionSearchService;
            _actionSummarySearchService = actionSummarySearchService;
            this._indexName = config.Value.PostIndexName;
            _settings = new IndexSettings()
            {
                //NumberOfReplicas = 0,
            };
            this._mapping = CreateContentPostMapping();
        }

        private TypeMapping CreateContentPostMapping()
        {
            var mapping = new TypeMapping();
            return mapping;
        }
        public override async Task<SearchResponse<ContentPostDocument>> GetAggregates(RequestQuery<ContentPostDocument> request, string sortByField = Constants.Fields.CreatedOn, SortOrder sortOrder = SortOrder.Asc)
        {
            return await base.GetAggregates(request);
        }
        public override async Task<SearchResponse<ContentPostDocument>> Find(RequestQuery<ContentPostDocument> query)
        {
            //Create if not existing
            await CreateIndex();
            //Search
            int take = query.ItemsPerPage > 0 ? query.ItemsPerPage : 10;
            int skip = take * query.Page;

            return await Search(
                CreateQuery(query),
                skip,
                take);
        }
        public async Task<DeleteIndexResponse> DeleteIndex()
        {
            return await base.DeleteIndex();
        }

        private QueryDescriptor<ContentPostDocument> CreateQuery(RequestQuery<ContentPostDocument> request)
        {
            var descriptor = new QueryDescriptor<ContentPostDocument>();
            descriptor.MatchAll();
            if (!string.IsNullOrEmpty(request?.Query?.Content))
                descriptor.Term(x => x.Content, request?.Query?.Content);

            return descriptor;
        }
        #region Posts
        // Posts Logic

        public QueryFilter PublicPosts()
        {
            return new QueryFilter()
            {
                Name = Foundations.Content.Constants.Fields.Status,
                Value = "public"
            };
        }
        public QueryFilter threadId(ElasticCondition cond, QueryOperator op)
        {
            return new QueryFilter()
            {
                Name = Foundations.Content.Constants.Fields.threadId,
                Condition = cond,
                Operator = op
            };
        }
        public QueryFilter SelfReplies()
        {
            return new QueryFilter()
            {
                Name = Foundations.Content.Constants.Fields.PostType,
                Condition = ElasticCondition.Must,
                Value = ContentPostType.Page
            };
        }
        public async Task<ContentSearchResults> SearchRelatedPosts(ContentChannelDocument channel, RequestQuery query, int page = 0, int itemsPerPage = 10)
        {
            var elasticQuery = BuildRelatedSearchQuery(channel, query);
            //TODO: remove

            int take = itemsPerPage > 0 ? itemsPerPage : ItemsPerPage;
            if (take > MaxQueryable) take = MaxQueryable;
            int skip = take * page;

            var model = new ContentSearchResults();
            var results = await Search(elasticQuery, skip, take);
            if (!results.IsValidResponse && !results.IsSuccess() || results.Documents == null)
            {
                return model;
            }
            model.Posts = results.Documents.ToList();
            return model;
        }
        public async Task<ContentSearchResults> SearchUserPosts(Guid userId, int page = 0, int itemsPerPage = 10)
        {
            var elasticQuery = BuildUserSearchQuery(userId);
            //TODO: remove

            int take = itemsPerPage > 0 ? itemsPerPage : ItemsPerPage;
            if (take > MaxQueryable) take = MaxQueryable;
            int skip = take * page;

            var model = new ContentSearchResults();
            var results = await Search(elasticQuery, skip, take);
            if (!results.IsValidResponse && !results.IsSuccess() || results.Documents == null)
            {
                return model;
            }
            model.Posts = results.Documents.ToList();
            return model;
        }
        public async Task<ContentSearchResults> SearchPosts(RequestQuery Query, QueryDescriptor<ContentPostDocument> elasticQuery, string sortField = Foundations.Integrations.Elastic.Constants.Fields.CreatedOn, Elastic.Clients.Elasticsearch.SortOrder sortOrder = Elastic.Clients.Elasticsearch.SortOrder.Desc)
        {
            //TODO: remove
            Query.ItemsPerPage = 100;

            int take = Query.ItemsPerPage > 0 ? Query.ItemsPerPage : ItemsPerPage;
            if (take > MaxQueryable) take = MaxQueryable;
            int skip = take * Query.Page;

            var model = new ContentSearchResults();
            sortField = !string.IsNullOrEmpty(sortField) ? sortField : Foundations.Integrations.Elastic.Constants.Fields.CreatedOn;
            var results = await Search(elasticQuery, skip, take, sortField ?? Foundations.Integrations.Elastic.Constants.Fields.CreatedOn, sortOrder);
            if (!results.IsValidResponse && !results.IsSuccess() || results.Documents == null)
            {
                return model;
            }
            model.Posts = results.Documents.ToList();

            //quotes
            var quotedPostIds = model.Posts.
                SelectMany(x => x.QuotedPosts.
                    Select(y => y.ContentPostQuoteId))
                .ToList();
            model.QuotedPosts = quotedPostIds.Any() ? await SearchPostQuotes(Query, quotedPostIds) : new List<ContentPostDocument>();

            //related entities
            var postIds = model.Posts.Select(x => x.Id.ToString()).ToList();
            model.Actions = await SearchPostActions(Query, postIds);
            model.ActionSummaries = await SearchPostActionSummaries(Query, postIds);

            return model;
        }
         
        public async Task<ContentSearchResults> SearchPosts(RequestQuery Query, string sortField = Foundations.Integrations.Elastic.Constants.Fields.CreatedOn, Elastic.Clients.Elasticsearch.SortOrder sortOrder = Elastic.Clients.Elasticsearch.SortOrder.Desc)
        {
            var elasticQuery = BuildSearchQuery(Query);
            return await SearchPosts(Query, elasticQuery, sortField, sortOrder);
        }
        public async Task<ContentSearchResults> SearchPost(RequestQuery Query, Guid postId, string sortField = Foundations.Integrations.Elastic.Constants.Fields.CreatedOn, Elastic.Clients.Elasticsearch.SortOrder sortOrder = Elastic.Clients.Elasticsearch.SortOrder.Desc)
        {
            Query.Page = 0;
            Query.ItemsPerPage = 1;
            var elasticQuery = BuildSearchPostQuery(Query, postId);
            return await SearchPosts(Query, elasticQuery, sortField, sortOrder);
        }
        public async Task<ContentSearchResults> SearchPostReplies(RequestQuery Query, string sortField = Foundations.Integrations.Elastic.Constants.Fields.CreatedOn, Elastic.Clients.Elasticsearch.SortOrder sortOrder = Elastic.Clients.Elasticsearch.SortOrder.Desc)
        {
            var elasticQuery = BuildSearchRepliesQuery(Query);
            return await SearchPosts(Query, elasticQuery, sortField, sortOrder);
        }
        private static Dictionary<Guid, ContentSearchResults> _parentThreadCache { get; set; } = new Dictionary<Guid, ContentSearchResults>();
        public async Task<ContentSearchResults> SearchPostParents(RequestQuery Query, Guid postId)
        {
            //if (_parentThreadCache.ContainsKey(postId)) return _parentThreadCache[postId];
            
            var response = await this.GetDocument<ContentPostDocument>(postId.ToString());
            var item = response.Source;
            var elasticQuery = BuildAscendantsSearchQuery(item);
            var results = await SearchPosts(Query, elasticQuery);
            results.Posts = results.Posts.OrderBy(x => x.CreatedOn);

            //related entities
            var postIds = results.Posts.Select(x => x.Id.ToString()).ToList();
            results.Actions = await SearchPostActions(Query, postIds);
            results.ActionSummaries = await SearchPostActionSummaries(Query, postIds);

            //_parentThreadCache.Add(postId, results);

            return results;
        }

        public RequestQuery<ContentPostDocument> CreateThreadAggregateQuery(Guid? threadId)
        {

            var filters = new List<QueryFilter>()
            {
                Filter(Foundations.Content.Constants.Fields.ParentId, ElasticCondition.Filter, threadId)
            };

            var aggregates = new List<string>()
            {
                Constants.Fields.threadId.ToCamelCase(),
                Constants.Fields.QuoteIds.ToCamelCase(),
                Constants.Fields.Tags.ToCamelCase(),
                Constants.Fields.Category.ToCamelCase(),
                Constants.Fields.ParentVote.ToCamelCase(),
            };
            return new RequestQuery<ContentPostDocument>() { Filters = filters, Aggregates = aggregates, ItemsPerPage = 100 };
        }
        #endregion
        public RequestQuery<ContentPostDocument> CreateAggregateQuery(Guid? threadId, List<QueryFilter> filters, List<string> fields)
        {
              return new RequestQuery<ContentPostDocument>() { Filters = filters, Aggregates = fields };
        }
        #region Quotes
        public async Task<List<ContentPostDocument>> SearchPostQuotes(RequestQuery Query, List<string> ids)
        {
            int take = Query.ItemsPerPage > 0 ? Query.ItemsPerPage : ItemsPerPage;
            if (take > MaxQueryable) take = MaxQueryable;
            int skip = take * Query.Page;
            var searchquery = BuildPostQuotesSearchQuery(Query, ids);
            var results = await this.Search(searchquery, skip, take);
            if (!results.IsValidResponse || !results.IsSuccess())
            {
                return new List<ContentPostDocument>();
            }
            return results.Documents.ToList();
        }
        public QueryDescriptor<ContentPostDocument> BuildPostQuotesSearchQuery(RequestQuery query, List<string> postIds)
        {
            var Query = new RequestQuery();
            Query.Filters = new List<QueryFilter>()
            {
                FilterValues(Constants.Fields.Id, ElasticCondition.Filter, QueryOperator.Equals, postIds, true)
            };
            return this.CreateQuery(Query);
        }
        #endregion
        #region ActionSummaries
        public async Task<List<ContentPostActionsSummaryDocument>> SearchPostActionSummaries(RequestQuery Query, List<string> ids)
        {
            int take = Query.ItemsPerPage > 0 ? Query.ItemsPerPage : ItemsPerPage;
            if (take > MaxQueryable) take = MaxQueryable;
            int skip = take * Query.Page;
            var searchquery = BuildPostActionsSummarySearchQuery(Query, ids);
            var results = await _actionSummarySearchService.Search<ContentPostActionsSummaryDocument>(searchquery, skip, take);
            if (!results.IsValidResponse || !results.IsSuccess())
            {
                return new List<ContentPostActionsSummaryDocument>();
            }
            return results.Documents.ToList();
        }
        public QueryDescriptor<ContentPostActionsSummaryDocument> BuildPostActionsSummarySearchQuery(RequestQuery query, List<string> postIds)
        {
            var Query = new RequestQuery();
            Query.Filters = new List<QueryFilter>()
            {
                FilterValues(Constants.Fields.ContentPostId, ElasticCondition.Filter, QueryOperator.Equals, postIds, true)
            };
            return _actionSummarySearchService.CreateQuery(Query);
        }
        #endregion
        #region Actions

        public async Task<List<ContentPostActionsDocument>> SearchPostActions(RequestQuery Query, List<string> ids)
        {
            int take = Query.ItemsPerPage > 0 ? Query.ItemsPerPage : ItemsPerPage;
            if (take > MaxQueryable) take = MaxQueryable;
            int skip = take * Query.Page;
            var searchquery = BuildPostActionsSearchQuery(Query, ids);
            var results = await _actionSearchService.Search<ContentPostActionsDocument>(searchquery, skip, take);
            if (!results.IsValidResponse || !results.IsSuccess())
            {
                return new List<ContentPostActionsDocument>();
            }
            return results.Documents.ToList();
        }
        public QueryDescriptor<ContentPostActionsDocument> BuildPostActionsSearchQuery(RequestQuery query, List<string> postIds)
        {
            var Query = new RequestQuery();
            Query.Filters = new List<QueryFilter>()
            {
                Filter(Constants.Fields.UserId, ElasticCondition.Filter, query.UserId),
                FilterValues(Constants.Fields.ContentPostId, ElasticCondition.Filter, QueryOperator.Equals, postIds, true)
            };
            return _actionSearchService.CreateQuery(Query);
        }
        public async Task<List<ContentPostActionsDocument>> SearchUserActions(RequestQuery Query)
        {
            var elasticQuery = GetUserActionsQuery(Query);
            int take = Query.ItemsPerPage > 0 ? Query.ItemsPerPage : ItemsPerPage;
            if (take > MaxQueryable) take = MaxQueryable;
            int skip = take * Query.Page;
            var results = await Search(elasticQuery, skip, take);
            if (!results.IsValidResponse || !results.IsSuccess())
            {
                return new List<ContentPostActionsDocument>();
            }
            return results.Documents.ToList();
        }
        public QueryDescriptor<ContentPostActionsDocument> GetUserActionsQuery(RequestQuery request)
        {
            var query = new QueryDescriptor<ContentPostActionsDocument>();
            if (request.Filters != null && request.Filters.Any())
            {
                query.MatchAll();
                query.Term(x =>
                    x.UserId.Suffix("keyword"),
                    request.Filters.FirstOrDefault(x => x.Name == Foundations.Content.Constants.Fields.UserId)
                );
            }
            return query;
        }
        #endregion

        #region Channels

        public async Task<List<ContentChannelDocument>> SearchChannels(RequestQuery Query)
        {
            var elasticQuery = GetChannelsQuery(Query);
            int take = Query.ItemsPerPage > 0 ? Query.ItemsPerPage : ItemsPerPage;
            if (take > MaxQueryable) take = MaxQueryable;
            int skip = take * Query.Page;
            var results = await Search(elasticQuery, skip, take);
            if (!results.IsValidResponse || !results.IsSuccess())
            {
                return new List<ContentChannelDocument>();
            }
            return results.Documents.ToList();
        }
        private QueryDescriptor<ContentChannelDocument> GetChannelsQuery(RequestQuery request)
        {
            var query = new QueryDescriptor<ContentChannelDocument>();
            if (request.Filters != null && request.Filters.Any())
            {
                query.MatchAll();
                query.Term(x =>
                    x.UserId.Suffix("keyword"),
                    request.Filters.FirstOrDefault(x => x.Name == Foundations.Content.Constants.Fields.UserId)
                );
            }
            return query;
        }
        public QueryDescriptor<ContentPostDocument> BuildRelatedSearchQuery(ContentChannelDocument channel, RequestQuery query)
        {
            var Query = new RequestQuery();
            Query.Filters.Add(Filter(Constants.Fields.Status, ElasticCondition.Must, "Public"));

            var notInChannel = Filter(Constants.Fields.ChannelId, ElasticCondition.MustNot, QueryOperator.NotEquals, channel.Id, true);
           
            notInChannel.Filters = new List<QueryFilter>()
            {
                FilterValues(Constants.Fields.Tags, ElasticCondition.Filter, QueryOperator.Equals, channel.Tags, true)
            };
            Query.Filters.Add(notInChannel);
            //Query.Filters.Add(Filter(Constants.Fields.Category, ElasticCondition.Filter, channel.Category));
            //Query.Filters.Add(FilterValues(Constants.Fields.Tags, ElasticCondition.Filter, QueryOperator.Equals, channel.Tags, true));

            return CreateQuery(Query);
        }


        public QueryDescriptor<ContentPostDocument> BuildUserSearchQuery(Guid userId)
        {
            var Query = new RequestQuery();
             
            Query.Filters.Add(Filter(Constants.Fields.UserId, ElasticCondition.Filter, userId));
             
            return CreateQuery(Query);
        }
        public List<string> ContentPostAggregateFields()
        {
            return new List<string>()
            {
                Foundations.Content.Constants.Fields.threadId.ToCamelCase(),
                Foundations.Content.Constants.Fields.Tags.ToCamelCase(),
            };
        }
        public RequestQuery<ContentPostDocument> CreateUserPostQuery(Guid userId)
        {

            var filters = new List<QueryFilter>()
            {
                this.Filter(Constants.Fields.UserId, ElasticCondition.Filter, userId)
            };
            var aggregates = ContentPostAggregateFields();
            return new RequestQuery<ContentPostDocument>() { Filters = filters, Aggregates = aggregates };
        }

        public override QueryDescriptor<ContentPostDocument> BuildSearchQuery(RequestQuery Query)
        {

            if (Query == null) Query = new RequestQuery();

            //Query.Filters.Add(PublicPosts());
            //Query.Filters.Add(Filter(Constants.Fields.Status, ElasticCondition.Must, "Public"));

            Query.Filters.Add(Filter(Constants.Fields.PostType, ElasticCondition.Filter, ContentPostType.Post));
           
            return CreateQuery(Query);
        }

        public QueryDescriptor<ContentPostDocument> BuildSearchPostQuery(RequestQuery Query, Guid postId)
        {

            if (Query == null) Query = new RequestQuery();

            //Query.Filters.Add(PublicPosts());
            //Query.Filters.Add(Filter(Constants.Fields.Status, ElasticCondition.Must, "Public"));

            //Filter any post where the poster is replying to themselves from the results
            var filter = Filter(Constants.Fields.Id, ElasticCondition.Filter, postId);
            Query.Filters.Add(filter);

            return CreateQuery(Query);
        }

        public QueryDescriptor<ContentPostDocument> BuildSearchRepliesQuery(RequestQuery Query)
        {

            if (Query == null) Query = new RequestQuery();

            //Query.Filters.Add(PublicPosts());
            //Query.Filters.Add(Filter(Constants.Fields.Status, ElasticCondition.Must, "Public"));

            //Filter any post where the poster is replying to themselves from the results
            var filter = Filter(Constants.Fields.PostType, ElasticCondition.Filter, ContentPostType.Reply);
            Query.Filters.Add(filter);
           
            return CreateQuery(Query);
        }

        public QueryDescriptor<ContentPostDocument> BuildAscendantsSearchQuery(ContentPostDocument item)
        {
            var Query = new RequestQuery()
            {
                Filters = new List<QueryFilter>()
            };

            Query.Filters.Add(Filter(Constants.Fields.Status, ElasticCondition.Must, "Public"));

            //Filter any post where the poster is replying to themselves from the results
            var ids = item.ParentIds != null ? item.ParentIds.Select(x => x.ToString()).ToList() : new List<string>();
            // add current item as well to retrieve associated properties (quotes/etc) of item
            ids.Add(item.Id.ToString());

            Query.Filters.Add(FilterValues(Foundations.Content.Constants.Fields.Id, ElasticCondition.Filter, QueryOperator.Equals, ids, true));
            
            //Query.Filters.Add(Filter(Constants.Fields.ParentIds, ElasticCondition.Filter, QueryOperator.Equals, item.ParentIds, true));

            return CreateQuery(Query);
        }
        public QueryDescriptor<ContentPostDocument> BuildRepliesSearchQuery(string parentId)
        {
            var Query = new RequestQuery()
            {
                Filters = new List<QueryFilter>()
            };

            //Query.Filters.Add(PublicPosts());
            Query.Filters.Add(Filter(Constants.Fields.Status, ElasticCondition.Must, "Public"));

            //Filter any post where the poster is replying to themselves from the results
            Query.Filters.Add(Filter(Constants.Fields.PostType, ElasticCondition.MustNot, ContentPostType.Page));
           
            Query.Filters.Add(Filter(Constants.Fields.ParentId, parentId));
           
            return CreateQuery(Query);
        }
        public RequestQuery<ContentPostDocument> CreateChannelAggregateQuery(Guid channelId)
        {

            var filters = new List<QueryFilter>()
            {
                this.Filter(Foundations.Content.Constants.Fields.ChannelId, ElasticCondition.Filter, channelId)
            };
            var aggregates = ContentPostAggregateFields();
            return new RequestQuery<ContentPostDocument>() { Filters = filters, Aggregates = aggregates };
        }

        public RequestQuery<ContentPostDocument> CreateChannelsAggregateQuery()
        {
            var filters = new List<QueryFilter>()
            {
                //this.Filter(Foundations.Content.Constants.Fields.channelId, ElasticCondition.Filter, channelId)
            };
            var aggregates = ContentPostAggregateFields();
            return new RequestQuery<ContentPostDocument>() { Filters = filters, Aggregates = aggregates };
        }

        public Elastic.Clients.Elasticsearch.QueryDsl.Query[] GetQueries(RequestQuery request, ElasticCondition condition)
        {
            throw new NotImplementedException();
        }


        #endregion

    }
}

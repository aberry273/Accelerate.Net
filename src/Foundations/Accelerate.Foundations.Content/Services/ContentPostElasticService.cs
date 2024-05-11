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

namespace Accelerate.Foundations.Content.Services
{
    //Overwrite the core service for custom filtering
    public class ContentPostElasticService : ElasticService<ContentPostDocument>, IContentPostElasticService
    {
        IElasticService<ContentPostActionsDocument> _actionSearchService;
        public ContentPostElasticService(
            IOptions<ElasticConfiguration> options, 
            IOptions<ContentConfiguration> config,
            IElasticService<ContentPostActionsDocument> actionSearchService) : base(options)
        {
            _actionSearchService = actionSearchService;
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
        public override async Task<SearchResponse<ContentPostDocument>> GetAggregates(RequestQuery<ContentPostDocument> request)
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
        public QueryFilter TargetThread(ElasticCondition cond, QueryOperator op)
        {
            return new QueryFilter()
            {
                Name = Foundations.Content.Constants.Fields.TargetThread,
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

            int take = itemsPerPage > 0 ? itemsPerPage : Foundations.Content.Constants.Search.DefaultPerPage;
            if (take > Foundations.Content.Constants.Search.MaxQueryable) take = Foundations.Content.Constants.Search.MaxQueryable;
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

            int take = itemsPerPage > 0 ? itemsPerPage : Foundations.Content.Constants.Search.DefaultPerPage;
            if (take > Foundations.Content.Constants.Search.MaxQueryable) take = Foundations.Content.Constants.Search.MaxQueryable;
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
        public async Task<ContentSearchResults> SearchPosts(RequestQuery Query)
        {
            var elasticQuery = BuildSearchQuery(Query);
            //TODO: remove
            Query.ItemsPerPage = 100;

            int take = Query.ItemsPerPage > 0 ? Query.ItemsPerPage : Foundations.Content.Constants.Search.DefaultPerPage;
            if (take > Foundations.Content.Constants.Search.MaxQueryable) take = Foundations.Content.Constants.Search.MaxQueryable;
            int skip = take * Query.Page;

            var model = new ContentSearchResults();
            var results = await Search(elasticQuery, skip, take);
            if (!results.IsValidResponse && !results.IsSuccess() || results.Documents == null)
            {
                return model;
            }
            model.Posts = results.Documents.ToList();
            //actions
            var postIds = model.Posts.Select(x => x.Id.ToString()).ToList();
            var postActionResults = await SearchPostActions(Query, postIds);
            model.Actions = postActionResults.ToList();
            return model;
        }

        public RequestQuery<ContentPostDocument> CreateThreadAggregateQuery(Guid? threadId)
        {

            var filters = new List<QueryFilter>()
            {
                Filter(Foundations.Content.Constants.Fields.ParentId, threadId)
            };

            var aggregates = new List<string>()
            {
                Foundations.Content.Constants.Fields.TargetThread.ToCamelCase(),
                Foundations.Content.Constants.Fields.Tags.ToCamelCase(),
            };
            return new RequestQuery<ContentPostDocument>() { Filters = filters, Aggregates = aggregates };
        }
        #endregion
        public RequestQuery<ContentPostDocument> CreateAggregateQuery(Guid? threadId, List<QueryFilter> filters, List<string> fields)
        {
              return new RequestQuery<ContentPostDocument>() { Filters = filters, Aggregates = fields };
        }
        #region Actions

        public async Task<List<ContentPostActionsDocument>> SearchPostActions(RequestQuery Query, List<string> ids)
        {
            int take = Query.ItemsPerPage > 0 ? Query.ItemsPerPage : Foundations.Content.Constants.Search.DefaultPerPage;
            if (take > Foundations.Content.Constants.Search.MaxQueryable) take = Foundations.Content.Constants.Search.MaxQueryable;
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
            int take = Query.ItemsPerPage > 0 ? Query.ItemsPerPage : Foundations.Content.Constants.Search.DefaultPerPage;
            if (take > Foundations.Content.Constants.Search.MaxQueryable) take = Foundations.Content.Constants.Search.MaxQueryable;
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
            int take = Query.ItemsPerPage > 0 ? Query.ItemsPerPage : Foundations.Content.Constants.Search.DefaultPerPage;
            if (take > Foundations.Content.Constants.Search.MaxQueryable) take = Foundations.Content.Constants.Search.MaxQueryable;
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
            //Query.Filters.Add(Filter(Constants.Fields.Status, ElasticCondition.Must, "Public"));

            var notInChannel = Filter(Constants.Fields.TargetChannel, ElasticCondition.MustNot, QueryOperator.Equals, channel.Id, true);
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

        public override QueryDescriptor<ContentPostDocument> BuildSearchQuery(RequestQuery Query)
        {

            if (Query == null) Query = new RequestQuery();

            //Query.Filters.Add(PublicPosts());
            //Query.Filters.Add(Filter(Constants.Fields.Status, ElasticCondition.Must, "Public"));

            // If searching for threads
            if (Query.Filters.Any(x => x.Name == Foundations.Content.Constants.Fields.ParentId))
            {
                //Filter any post where the poster is replying to themselves from the results
                var filter = Filter(Constants.Fields.PostType, ElasticCondition.Filter, ContentPostType.Reply);
                Query.Filters.Add(filter);
            }
            else
            {
                Query.Filters.Add(Filter(Constants.Fields.PostType, ElasticCondition.Filter, ContentPostType.Post));
            }
            return CreateQuery(Query);
        }
         
        public QueryDescriptor<ContentPostDocument> BuildRepliesSearchQuery(string threadId)
        {
            var Query = new RequestQuery()
            {
                Filters = new List<QueryFilter>()
            };

            //Query.Filters.Add(PublicPosts());
            Query.Filters.Add(Filter(Constants.Fields.Status, ElasticCondition.Must, "Public"));

            //Filter any post where the poster is replying to themselves from the results
            Query.Filters.Add(Filter(Constants.Fields.PostType, ElasticCondition.MustNot, ContentPostType.Page));
           
            Query.Filters.Add(Filter(Constants.Fields.ParentId, threadId));
           
            return CreateQuery(Query);
        }
        public RequestQuery<ContentPostDocument> CreateChannelAggregateQuery(Guid channelId)
        {

            var filters = new List<QueryFilter>()
            {
                this.Filter(Foundations.Content.Constants.Fields.TargetChannel, ElasticCondition.Filter, channelId)
            };
            var aggregates = new List<string>()
            {
                Foundations.Content.Constants.Fields.TargetThread.ToCamelCase(),
                Foundations.Content.Constants.Fields.Tags.ToCamelCase(),
            };
            return new RequestQuery<ContentPostDocument>() { Filters = filters, Aggregates = aggregates };
        }

        public RequestQuery<ContentPostDocument> CreateChannelsAggregateQuery()
        {
            var filters = new List<QueryFilter>()
            {
                //this.Filter(Foundations.Content.Constants.Fields.TargetChannel, ElasticCondition.Filter, channelId)
            };
            var aggregates = new List<string>()
            {
                Foundations.Content.Constants.Fields.TargetThread.ToCamelCase(),
                Foundations.Content.Constants.Fields.Tags.ToCamelCase(),
            };
            return new RequestQuery<ContentPostDocument>() { Filters = filters, Aggregates = aggregates };
        }

        public Elastic.Clients.Elasticsearch.QueryDsl.Query[] GetQueries(RequestQuery request, ElasticCondition condition)
        {
            throw new NotImplementedException();
        }


        #endregion

    }
}

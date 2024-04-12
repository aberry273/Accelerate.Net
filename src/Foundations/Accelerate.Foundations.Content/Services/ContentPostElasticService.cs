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

namespace Accelerate.Foundations.Content.Services
{
    //Overwrite the core service for custom filtering
    public class ContentPostElasticService : ElasticService<ContentPostDocument>, IContentPostElasticService
    {

        public ContentPostElasticService(IOptions<ElasticConfiguration> options) : base(options)
        {
            this._indexName = "contentpost_index";
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
                Value = ContentPostType.Thread
            };
        } 
        public async Task<List<ContentPostDocument>> SearchPosts(RequestQuery Query)
        {
            var elasticQuery = BuildSearchQuery(Query);
            //TODO: remove
            Query.ItemsPerPage = 100;

            int take = Query.ItemsPerPage > 0 ? Query.ItemsPerPage : Foundations.Content.Constants.Search.DefaultPerPage;
            if (take > Foundations.Content.Constants.Search.MaxQueryable) take = Foundations.Content.Constants.Search.MaxQueryable;
            int skip = take * Query.Page;

            var results = await Search(elasticQuery, skip, take);
            if (!results.IsValidResponse && !results.IsSuccess())
            {
                return new List<ContentPostDocument>();
            }
            return results.Documents.ToList();
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
        #region Reviews

        public async Task<List<ContentPostReviewDocument>> SearchUserReviews(RequestQuery Query)
        {
            var elasticQuery = GetUserReviewsQuery(Query);
            int take = Query.ItemsPerPage > 0 ? Query.ItemsPerPage : Foundations.Content.Constants.Search.DefaultPerPage;
            if (take > Foundations.Content.Constants.Search.MaxQueryable) take = Foundations.Content.Constants.Search.MaxQueryable;
            int skip = take * Query.Page;
            var results = await Search(elasticQuery, skip, take);
            if (!results.IsValidResponse || !results.IsSuccess())
            {
                return new List<ContentPostReviewDocument>();
            }
            return results.Documents.ToList();
        }
        public QueryDescriptor<ContentPostReviewDocument> GetUserReviewsQuery(RequestQuery request)
        {
            var query = new QueryDescriptor<ContentPostReviewDocument>();
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

        public override QueryDescriptor<ContentPostDocument> BuildSearchQuery(RequestQuery Query)
        {

            if (Query == null) Query = new RequestQuery();

            //Query.Filters.Add(PublicPosts());
            Query.Filters.Add(Filter(Constants.Fields.Status, ElasticCondition.Must, "Public"));

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

            // For any multi-select, apply the filter condition to each field
            if (Query.Filters.Any(x => x.Values != null && x.Values.Count > 0))
            {
                foreach (var filter in Query.Filters.Where(x => x.Values != null && x.Values.Count > 0))
                {
                    filter.Condition = ElasticCondition.Filter;
                }
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
            Query.Filters.Add(Filter(Constants.Fields.PostType, ElasticCondition.MustNot, ContentPostType.Thread));
           
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


        #endregion

    }
}

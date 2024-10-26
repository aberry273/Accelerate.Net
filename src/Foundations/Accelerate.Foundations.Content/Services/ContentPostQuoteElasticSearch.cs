using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Users.Models;
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
    public class ContentPostQuoteElasticService : ElasticService<ContentPostQuoteDocument>
    {

        public ContentPostQuoteElasticService(IOptions<ElasticConfiguration> options, IOptions<ContentConfiguration> config) : base(options)
        {
            this._indexName = config.Value.QuotesIndexName;
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
        public override async Task<SearchResponse<ContentPostQuoteDocument>> GetAggregates(RequestQuery<ContentPostQuoteDocument> request, string sortByField = Constants.Fields.CreatedOn, SortOrder sortOrder = SortOrder.Asc)
        {
            return await base.GetAggregates(request);
        }
        public override async Task<SearchResponse<ContentPostQuoteDocument>> Find(RequestQuery<ContentPostQuoteDocument> query)
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

        private QueryDescriptor<ContentPostQuoteDocument> CreateQuery(RequestQuery<ContentPostQuoteDocument> request)
        {
            var descriptor = new QueryDescriptor<ContentPostQuoteDocument>();
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
        public async Task<List<ContentPostQuoteDocument>> SearchPosts(RequestQuery Query)
        {
            var elasticQuery = BuildSearchQuery(Query);
            //TODO: remove
            Query.ItemsPerPage = 100;

            int take = Query.ItemsPerPage > 0 ? Query.ItemsPerPage : ItemsPerPage;
            if (take > MaxQueryable) take = MaxQueryable;
            int skip = take * Query.Page;

            var results = await Search(elasticQuery, skip, take);
            if (!results.IsValidResponse && !results.IsSuccess())
            {
                return new List<ContentPostQuoteDocument>();
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
                Foundations.Content.Constants.Fields.threadId.ToCamelCase(),
                Foundations.Content.Constants.Fields.Tags.ToCamelCase(),
            };
            return new RequestQuery<ContentPostDocument>() { Filters = filters, Aggregates = aggregates };
        }
        #endregion
        #region Actions

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

        public override QueryDescriptor<ContentPostQuoteDocument> BuildSearchQuery(RequestQuery Query)
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

            // For any multi-select, force the filter condition to each field to be Filter
            if (Query.Filters.Any(x => x.Values != null && x.Values.Any()))
            {
                foreach (var filter in Query.Filters.Where(x => x.Values != null && x.Values.Any()))
                {
                    filter.Condition = ElasticCondition.Filter;
                }
            }
            return CreateQuery(Query);
        }
          
        #endregion

    }
}

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
        /*
        public override async Task<IndexResponse> Index(ContentPostEntity doc)
        {
            //Create if not existing
            await CreateIndex();
            //Index
            return await IndexDocument(doc);
        }
        */
        /*
        public override async Task<ContentPostDocument> CreateIndex()
        {
            var response = await _client.Indices.CreateAsync<ContentPostDocument>(_indexName, c => c
               .Mappings(map => map
                   .Properties(p => p
                   .Text(t => t.ThreadIds)
                   .Object(o => o.EmployeeInfo, objConfig => objConfig
                       .Properties(p => p
                           .Text(t => t.EmployeeInfo.EmployeeName)
                           .Object(n => n.EmployeeInfo.JobRoles, objConfig => objConfig
                               .Properties(p => p
                                   .Text(t => t.EmployeeInfo.JobRoles.First().RoleName)
                               )
                           )
                       )
                   )
               )
           ));
            return response;
        }
        */
        public override async Task<SearchResponse<ContentPostDocument>> GetAggregates(RequestQuery<ContentPostDocument> request)
        {
            //Create if not existing
            await CreateIndex();
            int take = request.ItemsPerPage > 0 ? request.ItemsPerPage : 10;
            int skip = take * request.Page;
            var query = this.BuildSearchQuery(request);
            //
             
            return await _client.SearchAsync<ContentPostDocument>(s => s
                .Index(_indexName)
                .Query(query)
                .Aggregations(a => a
                    .Terms("tags", t => t
                        .Field(f => f.Tags.Suffix("keyword"))
                        .Size(10000)
                    )
                    .Terms("threadIds", t => t
                        .Field(f => f.ThreadId.Suffix("keyword"))
                        .Size(10000)
                    )
                )
            );
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
                Name = Foundations.Content.Constants.Fields.SelfReply,
                Condition = ElasticCondition.Must,
                Operator = QueryOperator.Exist,
                //ValueType = Common.Models.ValueType.False
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
        private QueryDescriptor<ContentPostReviewDocument> GetUserReviewsQuery(RequestQuery request)
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
            if (Query.Filters.Any(x => x.Name == Foundations.Content.Constants.Fields.TargetThread))
            {
                //Filter any post where the poster is replying to themselves from the results
                Query.Filters.Add(Filter(Constants.Fields.SelfReply, false));
            }
            else
            {
                Query.Filters.Add(Filter(Constants.Fields.TargetThread, ElasticCondition.MustNot, QueryOperator.Exist));
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


        #endregion
    }
}

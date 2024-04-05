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
            this._mapping = new TypeMapping()
            { 
            };
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
        public override async Task<SearchResponse<ContentPostDocument>> GetAggregates(RequestQuery<ContentPostDocument> query)
        {

            //Create if not existing
            await CreateIndex();
            int take = query.ItemsPerPage > 0 ? query.ItemsPerPage : 10;
            int skip = take * query.Page;
            //
            return await _client.SearchAsync<ContentPostDocument>(s => s
                .Index(_indexName)
                .Query(CreateQuery(query))
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
            if (Query == null) Query = new RequestQuery();

            //Query.Filters.Add(PublicPosts());
            Query.Filters.Add(Filter(Constants.Fields.Status, ElasticCondition.Must, QueryOperator.Equals, "Public"));

            if (Query.Filters.Any(x => x.Name == Foundations.Content.Constants.Fields.TargetThread))
            {
                Query.Filters.Add(Filter(Constants.Fields.SelfReply, true));
            }
            else
            {
                Query.Filters.Add(Filter(Constants.Fields.TargetThread, ElasticCondition.MustNot, QueryOperator.Exist));
            }

            Query.Filters = new List<QueryFilter>();
            Query.Filters.Add(Filter(Constants.Fields.TargetThread, ElasticCondition.Must, QueryOperator.Exist));

            Query.Filters.Add(new QueryFilter()
            {
                Name = Constants.Fields.TargetThread,
                Value = "PAnb4Lw6NEi63Mlrw6Wz2g",
                Keyword = true
            });


            var elasticQuery = CreateQuery(Query);
      
            elasticQuery = new QueryDescriptor<ContentPostDocument>();
           
            elasticQuery.MatchAll();
         
            elasticQuery.Terms(t => t
                   .Field(Foundations.Content.Constants.Fields.threadId)
                   .Terms(new TermsQueryField(new FieldValue[] { FieldValue.String("PAnb4Lw6NEi63Mlrw6Wz2g") })).Suffix("keyword")
               );
         
            elasticQuery.Bool(x => x.Must(y => y.Exists(z => z.Field(Foundations.Content.Constants.Fields.TargetThread))));
           
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
        

        /*

        // Tags
        if (request.Filters.ContainsKey(Foundations.Content.Constants.Fields.Tags))
        {
            var tagValues = request
               .Filters[Foundations.Content.Constants.Fields.Tags]
               .Select(FieldValue.String).ToArray();
            query
               .Bool(b => b 
                   .Filter(f => f
                       .Bool(bb => bb
                           .Must(m => m
                               .Terms(t => t
                                   .Field(Foundations.Content.Constants.Fields.Tags.ToLower())
                                   .Terms(new TermsQueryField(tagValues))
                               )
                           )
                       )
                    )
                );
        }

        if (request.Filters.ContainsKey(Foundations.Content.Constants.Fields.Threads))
        {
            var tagValues = request
               .Filters[Foundations.Content.Constants.Fields.Threads]
               .Select(FieldValue.String).ToArray();
            query
               .Bool(b => b
                   .Filter(f => f
                       .Bool(bb => bb
                           .Must(m => m
                               .Terms(t => t
                                   .Field(Foundations.Content.Constants.Fields.TargetThread)
                                   .Terms(new TermsQueryField(tagValues)).Suffix("keyword")
                               )
                           )
                       )
                    )
                );
        }

        if (request.Filters.ContainsKey(Foundations.Content.Constants.Fields.TargetThread))
        {
            var threadValues = request
                .Filters[Foundations.Content.Constants.Fields.TargetThread]
                .Select(FieldValue.String).ToArray();

            query
                .Bool(b => b
                /*
                   .Filter(f => f
                       .Bool(bb => bb
                           .Must(m => m
                               .Terms(t => t
                                   .Field(Foundations.Content.Constants.Fields.TargetThread)
                                   .Terms(new TermsQueryField(threadValues)).Suffix("keyword")
                               )
                               .Term(t => t
                                   .Field(Foundations.Content.Constants.Fields.SelfReply)
                                   .Value(0)
                               )
                           )
                        )
                    )
               */
            /*
               .Should(f => f
                   .Term(t => t
                       .Field(Foundations.Content.Constants.Fields.SelfReply)
                       .Value(FieldValue.Boolean(false))
                   )
                   .Terms(t => t
                       .Field(Foundations.Content.Constants.Fields.TargetThread)
                       .Terms(new TermsQueryField(threadValues)).Suffix("keyword")
                   ) 
               )

            .Must(m => m
                .Terms(t => t
                    .Field(Foundations.Content.Constants.Fields.TargetThread)
                    .Terms(new TermsQueryField(threadValues)).Suffix("keyword")
                )
                .Term(t => t
                    .Field(Foundations.Content.Constants.Fields.SelfReply)
                    .Value(FieldValue.Boolean(false))
                )
            )

            .Term(x => 
                x.SelfReply,
                FieldValue.Boolean(false)
            ) 
            )
            }
            else
            {
                //query.Bool(x => x.MustNot(y => y.Exists(z => z.Field(Foundations.Content.Constants.Fields.TargetThread))));
            }
                    */
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
    }
}

using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Aggregations;
using Elastic.Clients.Elasticsearch.Core.MSearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks; 

namespace Accelerate.Foundations.Integrations.Elastic.Services
{
    public abstract class ElasticService<T> : IElasticService<T>
    {
        ElasticConfiguration _config;
        protected ElasticsearchClient _client;
        public ElasticsearchClient Client
        {
            get
            {
                return _client;
            }
        }
        public string IndexName
        {
            get
            {
                return _indexName;
            }
        }
        protected string _indexName { get; set; }
        protected TypeMapping _mapping { get; set; } = new TypeMapping();
        protected IndexSettings _settings { get; set; } = new IndexSettings();
        public ElasticService(IOptions<ElasticConfiguration> options)
        {
            _config = options.Value;
            _client = new ElasticsearchClient(_config.CloudId, new ApiKey(_config.ApiKey));
        }
        
        public async Task<CreateIndexResponse> CreateIndex()
        {
            return await _client.Indices.CreateAsync<T>(
                _indexName
                ,x => x.Mappings(_mapping).Settings(_settings)
          );
        }
        public async Task<DeleteIndexResponse> DeleteIndex()
        {
            return await _client.Indices.DeleteAsync(_indexName);
        }
        public async Task<IndexResponse> Index(T doc)
        {
            //Create if not existing
            await CreateIndex();
            //Index
            return await IndexDocument(doc);
        }
        public async Task<IndexResponse> IndexDocument<T>(T document)
        {
            //Create if not existing
            await CreateIndex();
            return await _client.IndexAsync(document, _indexName);
        }
        public async Task<CountResponse> Count<T>(Action<CountRequestDescriptor<T>> request)
        {
            return await _client.CountAsync<T>(request);
        }
        
        /// <summary>
        /// TODO
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<MultiSearchResponse<T>> Multisearch<T>(MultiSearchRequestDescriptor<T> request)
        {
            /*
           var query = new Query(query=> query.Term);
           query.Term(x =>
                  x.TargetThread.Suffix("keyword"),
                  request.Filters[Foundations.Content.Constants.Fields.TargetThread]?.FirstOrDefault()
              );
           */
            Query q = new TermQuery("contentPostId") { Value = "" };
            Query q1 = new TermQuery("agree") { Value = true };
            Query q2 = new TermQuery("disagree") { Value = true };
            Query q3 = new TermQuery("like") { Value = true };
       
            /*
            client.MultiSearch(ms => ms
                .Search<ElasticsearchProject>("projects", s => s.MatchAll())
                .Search<Person>("people", s => s.MatchAll())
            );
            */
            /*
            Func<CountRequestDescriptor<dynamic>, CountRequest> countQueryFilter = q => q.Query(q =>
               q.Match(m => m.Field("volumeId").Query("vol.e144f0bc59914725528f08d995ebd8c3"))
               && q.Match(m => m.Field("dataType").Query("File")) &&
               q.Prefix(m => m.Field("path.raw").Value($"{folderPrefix}")));
            */

            var ms = new MultiSearchRequestDescriptor<T>();
            var searchRequestItem = new SearchRequestItem(
                new MultisearchHeader { Index = _indexName },
                new MultisearchBody { Query = q }
            );
            ms.AddSearch(searchRequestItem);

            return await _client.MultiSearchAsync<T>(request);
        }

        public async Task<MultiGetResponse<T>> GetDocuments<T>(List<Guid> ids)
        {
            var multiget = new MultiGetRequest(IndexName)
            {
                Ids = new Ids(ids.Select(x => x.ToString()))
            };
            var response = await _client.MultiGetAsync<T>(multiget);
            var result = response.Docs.FirstOrDefault();
            return response;
        }
        public async Task<GetResponse<T>> GetDocument<T>(string id)
        {
            return await _client.GetAsync<T>(id, idx => idx.Index(_indexName));
        }
        public async Task<UpdateResponse<T>> UpdateDocument<T>(T document, string id)
        {
            //Create if not existing
            await CreateIndex();
            return await _client.UpdateAsync<T, T>(_indexName, id, x => x.Doc(document));
        }
        public async Task<UpdateResponse<T>> UpdateOrCreateDocument<T>(T document, string id)
        {
            //Create if not existing
            await CreateIndex();
            var respone = await _client.UpdateAsync<T, T>(_indexName, id, x => x.Doc(document));
            if (!respone.IsValidResponse && respone.ApiCallDetails.HttpStatusCode == 404)
            {
                var createResponse = await this.IndexDocument<T>(document);
                //respone.IsValidResponse = createResponse.IsValidResponse;
            }
            return respone;
        }
        public async Task<DeleteResponse> DeleteDocument<T>(string id)
        {
            return await _client.DeleteAsync(_indexName, id);
        }
        public async Task<SearchResponse<T>> Search<T>(QueryDescriptor<T> query, int from = 0, int take = 10, string sortByField = Constants.Fields.CreatedOn)
        {
            return await _client.SearchAsync<T>(s => s
                .Index(_indexName)
                .From(from)
                .Size(take)
                .Query(query)
                .Sort(x => x.Field(new Field(sortByField)))
            );
        }
        public async Task<SearchResponse<T>> Search<T>(Query query, int from = 0, int take = 10, string sortByField = Constants.Fields.CreatedOn)
        {
            return await _client.SearchAsync<T>(s => s
                .Index(_indexName)
                .From(from)
                .Size(take)
                .Query(query)
                .Sort(x => x.Field(new Field(sortByField)))
            );
        }

        public async Task<SearchResponse<T>> Search<T>(Action<QueryDescriptor<T>> query, int from = 0, int take = 10, string sortByField = Constants.Fields.CreatedOn)
        {
            return await _client.SearchAsync<T>(s => s
                .Index(_indexName)
                .From(from)
                .Size(take)
                .Query(query)
                .Sort(x => x.Field(new Field(sortByField)))
            );
        }
        public async Task<SearchResponse<T>> Find(QueryDescriptor<T> query, int page = 0, int itemsPerPage = 10, string sortByField = Constants.Fields.CreatedOn)
        {
            //Create if not existing
            await CreateIndex();
            //Search
            int take = itemsPerPage > 0 ? itemsPerPage : 10;
            int skip = take * page;

            return await Search(
                query,
                skip,
                take,
                sortByField);
        }
        public async Task<SearchResponse<T>> Find(BoolQuery query, int page = 0, int itemsPerPage = 10, string sortByField = Constants.Fields.CreatedOn)
        {
            //Create if not existing
            await CreateIndex();
            //Search
            int take = itemsPerPage > 0 ? itemsPerPage : 10;
            int skip = take * page;

            return await _client.SearchAsync<T>(s => s
                .Index(_indexName)
                .From(skip)
                .Size(take)
                .Query(query)
                .Sort(x => x.Field(new Field(sortByField)))
            );
        }
        // Overrides
        public abstract Task<SearchResponse<T>> Find(RequestQuery<T> query);
        
        public async virtual Task<SearchResponse<T>> GetAggregates(RequestQuery<T> request)
        {
            //Create if not existing
            await CreateIndex();
            int take = request.ItemsPerPage > 0 ? request.ItemsPerPage : 10;
            int skip = take * request.Page;
            var query = this.CreateQuery(request);
            //
            var aggregates = request.Aggregates ?? new List<string>();
            Action<AggregationDescriptor<T>> disciptors = (AggregationDescriptor<T> a) =>
            {
                foreach (var field in aggregates)
                {
                    a.Terms(field, t => t
                            .Field($"{field}.keyword")
                            .Size(10000)
                        );
                }
            };
            return await _client.SearchAsync<T>(s => s
                .Index(_indexName)
                .Query(query)
                .Aggregations(disciptors)
            );
        }
        
        // Custom 
        public FieldValue GetFieldValue(QueryFilter filter, object? value)
        {
            switch (filter.ValueType)
            {
                case Common.Models.ValueType.String:
                    return FieldValue.String(value?.ToString());
                case Common.Models.ValueType.Boolean:
                    return FieldValue.Boolean((bool)value);
                case Common.Models.ValueType.Double:
                    return FieldValue.Long((long)value);
                case Common.Models.ValueType.Null:
                    return FieldValue.Null;
                case Common.Models.ValueType.False:
                    return FieldValue.False;
                case Common.Models.ValueType.True:
                    return FieldValue.True;
                default:
                    return FieldValue.String(value?.ToString());
            }
        }
        public FieldValue GetFieldValue(QueryFilter filter)
        {
            return GetFieldValue(filter, filter.Value);
        }

        public Query? CreateTermsQuery(QueryFilter filter)
        {
            if (!filter.Values.Any()) return null;
            var name = filter.Name.ToCamelCase();
            if (filter.Keyword) { name += ".keyword"; }
            var values = filter.Values
                    .Select(x => GetFieldValue(filter, x))
                    .ToArray();

            var q = new TermsQuery()
            {
                Field = new Field(name),
                Terms = new TermsQueryField(values),
            };
            return q;
        }
        public Query? CreateTermQuery(QueryFilter filter)
        {
            var name = filter.Name.ToCamelCase();
            //Udpate mappings to include keyword analyzer instead
            if (filter.Keyword) { name += ".keyword"; }
            var field = new Field(name);
            var tq = new TermQuery(field)
            {
                Value = GetFieldValue(filter),
                CaseInsensitive = false
            };
            return tq;
        }
        public Query? CreatExistsQuery(QueryFilter filter)
        {
            var name = filter.Name.ToCamelCase();
            var field = new Field(name);
            var q = new ExistsQuery();
            q.Field = field;
            q.Suffix("keyword");
            return q;
        }
        public QueryFilter Filter(string field, object? value)
        {
            return new QueryFilter()
            {
                Name = field,
                Value = value
            };
        }
        public QueryFilter Filter(string field, ElasticCondition cond, object? value)
        {
            return new QueryFilter()
            {
                Name = field,
                Condition = cond,
                Value = value
            };
        }
        public QueryFilter Filter(string field, ElasticCondition cond, QueryOperator op)
        {
            return new QueryFilter()
            {
                Name = field,
                Condition = cond,
                Operator = op
            };
        }
        public QueryFilter Filter(string field, ElasticCondition cond, QueryOperator op, object? value, bool? keyword)
        {
            return new QueryFilter()
            {
                Name = field,
                Condition = cond,
                Operator = op,
                Value = value,
                Keyword = keyword.GetValueOrDefault(),
            };
        }
        public QueryFilter FilterValues(string field, ElasticCondition cond, QueryOperator op, IEnumerable<object>? values, bool? keyword)
        {
            return new QueryFilter()
            {
                Name = field,
                Condition = cond,
                Operator = op,
                Values = values,
                Keyword = keyword.GetValueOrDefault(),
            };
        }

        public Query? CreateTerm(QueryFilter? filter)
        {
            if (filter.Value != null)
            {
                return CreateTermQuery(filter);
            }
            else if (filter.Values != null && filter.Values.Any())
            {
                return CreateTermsQuery(filter);
            }
            else if (filter.Operator == QueryOperator.Exist)
            {
                return CreatExistsQuery(filter);
            }
            else
            {
                return null;
            }
        }
        public Query[] GetQueries(List<QueryFilter> filters, ElasticCondition condition)
        {
            return filters?.
                Where(x => x.Condition == condition).
                Select(CreateTerm).
                Where(x => x != null).
                ToArray() ?? new Query[0];
        }
        public QueryDescriptor<T> CreateQuery(RequestQuery request)
        {
            var query = new QueryDescriptor<T>();
            query.MatchAll();
           // query = CreateQuery(new QueryDescriptor<T>(), request.Filters);
         
            var must = GetQueries(request?.Filters, ElasticCondition.Must);
            if (must.Any()) query.Bool(x => x.Must(must));
            
            var mustNot = GetQueries(request?.Filters, ElasticCondition.MustNot);
            if (mustNot.Any()) query.Bool(x => x.MustNot(mustNot));

            var should = GetQueries(request?.Filters, ElasticCondition.Should);
            if (should.Any()) query.Bool(x => x.Should(should));

            var filter = GetQueries(request?.Filters, ElasticCondition.Filter);
            if (filter.Any()) query.Bool(x => x.Filter(filter));
            
            return query;
        }

        public virtual QueryDescriptor<T> BuildSearchQuery(RequestQuery Query)
        {
            if (Query == null) Query = new RequestQuery();

            return CreateQuery(Query);
        }
        /// <summary>
        /// Main search query that uses the BuildSearchQuery function to apply document logic search for ElasticSearch
        /// </summary>
        /// <param name="Query"></param>
        /// <returns></returns>
        public async Task<List<T>> Search(RequestQuery Query)
        {
            var elasticQuery = BuildSearchQuery(Query);
            //TODO: remove
            Query.ItemsPerPage = 100;

            int take = Query.ItemsPerPage > 0 ? Query.ItemsPerPage : Constants.Search.DefaultPerPage;
            if (take > Constants.Search.MaxQueryable) take = Constants.Search.MaxQueryable;
            int skip = take * Query.Page;

            var results = await Search(elasticQuery, skip, take);
            if (!results.IsValidResponse && !results.IsSuccess())
            {
                return new List<T>();
            }
            return results.Documents.ToList();
        }

    }
}

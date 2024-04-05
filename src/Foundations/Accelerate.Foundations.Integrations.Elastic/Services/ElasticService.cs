using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Common.Models;
using Elastic.Clients.Elasticsearch;
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
            return await _client.Indices.CreateAsync(
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
        // Overrides
        public abstract Task<SearchResponse<T>> Find(RequestQuery<T> query);
        public abstract Task<SearchResponse<T>> GetAggregates(RequestQuery<T> query);

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

        public Query CreateTermsQuery(QueryFilter filter)
        {
            var name = filter.Name;
            var values = filter.Values
                    .Select(x => GetFieldValue(filter, x))
                    .ToArray();
            /*
            var tq = new TermQueryDescriptor<ContentPostDocument>(doc =>
                values.Contains(doc.TargetThread));
            */

            //                    .Name(name).Value(new TermsQueryField(threadValues)).Suffix("keyword"));
            /*

                .Field(Foundations.Content.Constants.Fields.TargetThread)
                .Terms(new TermsQueryField(threadValues)).Suffix("keyword")
            */
            var q = new TermsQuery()
            {
                Field = new Field(name),
                Terms = new TermsQueryField(values),
            };
            if (filter.Keyword)
            {
                q.Terms.Suffix("keyword");
            }
            return q;
        }
        public Query CreateTermQuery(QueryFilter filter)
        {
            var name = filter.Name;
            var field = new Field(name);
          
            field.Suffix("keyword");
            var tq = new TermQuery(field)
            {
                Field = new Field(name),
                Value = GetFieldValue(filter),
                CaseInsensitive = false
            };
            if(filter.Keyword)
            {
             //  tq.Suffix("keyword");
            }
            return tq;
        }
        public Query CreatExistsQuery(QueryFilter filter)
        {
            var name = filter.Name;
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
        public QueryFilter Filter(string field, ElasticCondition cond, QueryOperator op)
        {
            return new QueryFilter()
            {
                Name = field,
                Condition = cond,
                Operator = op
            };
        }
        public QueryFilter Filter(string field, ElasticCondition cond, QueryOperator op, object? value)
        {
            return new QueryFilter()
            {
                Name = field,
                Condition = cond,
                Operator = op,
                Value = value
            };
        }
        public QueryFilter Filter(string field, ElasticCondition cond, QueryOperator op, List<object>? values)
        {
            return new QueryFilter()
            {
                Name = field,
                Condition = cond,
                Operator = op,
                Values = values
            };
        }

        public Query? CreateTerm(QueryFilter filter)
        {
            if (filter.Value != null)
            {
                return CreateTermQuery(filter);
            }
            else if (filter.Values != null && filter.Values.Count > 0)
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
        public Query[] GetQueries(RequestQuery request, ElasticCondition condition)
        {
            return request?.Filters?.
                Where(x => x.Condition == condition).
                Select(CreateTerm).
                Where(x => x != null).
                ToArray() ?? new Query[0];
        }
        public QueryDescriptor<T> CreateQuery(RequestQuery request)
        {
            var query = new QueryDescriptor<T>();
            query.MatchAll();

            var must = GetQueries(request, ElasticCondition.Must);
            if (must.Any()) query.Bool(x => x.Must(must));
            
            var mustNot = GetQueries(request, ElasticCondition.MustNot);
            if (mustNot.Any()) query.Bool(x => x.MustNot(mustNot));

            var should = GetQueries(request, ElasticCondition.Should);
            if (should.Any()) query.Bool(x => x.Should(should));

            var filter = GetQueries(request, ElasticCondition.Filter);
            if (filter.Any()) query.Bool(x => x.Filter(filter));

            /*
            var threadValues = request
                .Filters.FirstOrDefault(x => x.Name==Foundations.Content.Constants.Fields.TargetThread)             
                .Values.Select(y=> FieldValue.String(y.ToString())).ToArray();

            query.Bool(b => b 
                .Must(m => m
                    .Terms(t => t

                    .Field(Foundations.Content.Constants.Fields.TargetThread)
                    .Terms(new TermsQueryField(threadValues)).Suffix("keyword")
)
               ));
            */
            /*
            foreach(var filter in request.Filters)
            {
                var name = filter.Name;
                if (filter.Value != null)
                {
                    var value = FieldValue.String(filter.Value.ToString());

                    query.Term(t => t
                        .Field(name)
                        .Value(value)
                        //.Suffix("keyword")
                    );
                }
                else if (filter.Values != null && filter.Values.Count > 0)
                {
                    var values = filter.Values.Select(x => FieldValue.String(x.ToString())).ToArray();

                    query.Terms(t => t
                        .Field(name)
                        .Terms(new TermsQueryField(values))
                        //.Suffix("keyword")
                    );
                }
                else if (filter.Operator == QueryOperator.Null)
                {
                    query.Bool(x => x.MustNot(y => y.Exists(z => z.Field(name).Suffix("keyword"))));
                }
            }
            */
            return query;
        }
    }
}

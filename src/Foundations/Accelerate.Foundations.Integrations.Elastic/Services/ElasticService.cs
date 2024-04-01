using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Common.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.MSearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
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
        public ElasticService(IOptions<ElasticConfiguration> options)
        {
            _config = options.Value;
            _client = new ElasticsearchClient(_config.CloudId, new ApiKey(_config.ApiKey));
        }
        
        public async Task<CreateIndexResponse> CreateIndex()
        {
            return await _client.Indices.CreateAsync(_indexName);
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
            return await _client.UpdateAsync<T, T>(_indexName, id, x => x.Doc(document));
        }
        public async Task<UpdateResponse<T>> UpdateOrCreateDocument<T>(T document, string id)
        {
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
        public async Task<SearchResponse<T>> Search<T>(QueryDescriptor<T> query, int from = 0, int take = 10, string sortByField = Constants.Fields.UpdatedOn)
        {
            return await _client.SearchAsync<T>(s => s
                .Index(_indexName)
                .From(from)
                .Size(take)
                .Query(query)
                .Sort(x => x.Field(new Field(sortByField)))
            );
        }
        public async Task<SearchResponse<T>> Search<T>(Query query, int from = 0, int take = 10, string sortByField = Constants.Fields.UpdatedOn)
        {
            return await _client.SearchAsync<T>(s => s
                .Index(_indexName)
                .From(from)
                .Size(take)
                .Query(query)
                .Sort(x => x.Field(new Field(sortByField)))
            );
        }

        public async Task<SearchResponse<T>> Search<T>(Action<QueryDescriptor<T>> query, int from = 0, int take = 10, string sortByField = Constants.Fields.UpdatedOn)
        {
            return await _client.SearchAsync<T>(s => s
                .Index(_indexName)
                .From(from)
                .Size(take)
                .Query(query)
                .Sort(x => x.Field(new Field(sortByField)))
            );
        }
        public async Task<SearchResponse<T>> Find(QueryDescriptor<T> query, int page = 0, int itemsPerPage = 10, string sortByField = Constants.Fields.UpdatedOn)
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
    }
}

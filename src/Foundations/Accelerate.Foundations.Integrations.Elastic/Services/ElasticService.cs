using Accelerate.Foundations.Account.Models;
using Elastic.Clients.Elasticsearch;
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
    public class ElasticService : IElasticService
    {
        ElasticConfiguration _config;
        protected ElasticsearchClient _client;
         
        public ElasticService(IOptions<ElasticConfiguration> options)
        {
            _config = options.Value;
            _client = new ElasticsearchClient(_config.CloudId, new ApiKey(_config.ApiKey));
        }
        
        public async Task<CreateIndexResponse> CreateIndex(string indexName)
        {
            return await _client.Indices.CreateAsync(indexName);
        }
        public async Task<DeleteIndexResponse> DeleteIndex(string indexName)
        {
            return await _client.Indices.DeleteAsync(indexName);
        }
        public async Task<IndexResponse> IndexDocument<T>(T document, string index)
        {
            return await _client.IndexAsync(document, index);
        }
        public async Task<GetResponse<T>> GetDocument<T>(string id, string index)
        {
            return await _client.GetAsync<T>(id, idx => idx.Index(index));
        }
        public async Task<UpdateResponse<T>> UpdateDocument<T>(T document, string id, string index)
        {
            return await _client.UpdateAsync<T, T>(index, id, x => x.Doc(document));
        }
        public async Task<DeleteResponse> DeleteDocument<T>(string id, string index)
        {
            return await _client.DeleteAsync(index, id);
        }
        public async Task<SearchResponse<T>> SearchDocuments<T>(string index, QueryDescriptor<T> query)
        {
            return await _client.SearchAsync<T>(s => s
                .Index(index)
                .From(0)
                .Size(10)
                .Query(query)
                );
        }
    }
}

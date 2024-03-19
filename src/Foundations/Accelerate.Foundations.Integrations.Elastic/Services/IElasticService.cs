using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace Accelerate.Foundations.Integrations.Elastic.Services
{
    public interface IElasticService
    {
        Task<CreateIndexResponse> CreateIndex(string indexName);
        Task<DeleteIndexResponse> DeleteIndex(string indexName);
        Task<IndexResponse> IndexDocument<T>(T document, string index);
        Task<GetResponse<T>> GetDocument<T>(string id, string index);
        Task<UpdateResponse<T>> UpdateDocument<T>(T document, string id, string index);
        Task<DeleteResponse> DeleteDocument<T>(string id, string index);
        Task<SearchResponse<T>> SearchDocuments<T>(string index, QueryDescriptor<T> query);
    }
}

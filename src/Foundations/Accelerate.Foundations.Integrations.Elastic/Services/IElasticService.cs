using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Accelerate.Foundations.Common.Models;

namespace Accelerate.Foundations.Integrations.Elastic.Services
{
    public interface IElasticService<T>
    {
        Task<CreateIndexResponse> CreateIndex();
        Task<DeleteIndexResponse> DeleteIndex();
        Task<IndexResponse> IndexDocument<T>(T document);
        Task<GetResponse<T>> GetDocument<T>(string id);
        Task<UpdateResponse<T>> UpdateDocument<T>(T document, string id);
        Task<DeleteResponse> DeleteDocument<T>(string id);
        Task<SearchResponse<T>> SearchDocuments<T>(QueryDescriptor<T> query, int from = 0, int take = 10);

        Task<SearchResponse<T>> Find(RequestQuery<T> query);
        Task<IndexResponse> Index(T doc);
    }
}

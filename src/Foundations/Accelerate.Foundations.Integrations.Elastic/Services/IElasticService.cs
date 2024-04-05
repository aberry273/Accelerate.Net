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
        ElasticsearchClient Client { get; }
        string IndexName { get; }
        Task<CreateIndexResponse> CreateIndex();
        Task<DeleteIndexResponse> DeleteIndex();
        Task<IndexResponse> IndexDocument<T>(T document);
        Task<GetResponse<T>> GetDocument<T>(string id);
        Task<UpdateResponse<T>> UpdateDocument<T>(T document, string id);
        Task<UpdateResponse<T>> UpdateOrCreateDocument<T>(T document, string id);
        Task<DeleteResponse> DeleteDocument<T>(string id);
        Task<MultiSearchResponse<T>> Multisearch<T>(MultiSearchRequestDescriptor<T> request);
        Task<SearchResponse<T>> Search<T>(QueryDescriptor<T> query, int from = 0, int take = 10, string sortByField = Constants.Fields.CreatedOn);
        Task<SearchResponse<T>> Search<T>(Query query, int from = 0, int take = 10, string sortByField = Constants.Fields.CreatedOn);
        Task<SearchResponse<T>> Search<T>(Action<QueryDescriptor<T>> query, int from = 0, int take = 10, string sortByField = Constants.Fields.CreatedOn);
        Task<IndexResponse> Index(T doc);
        Task<CountResponse> Count<T>(Action<CountRequestDescriptor<T>> request);

        Task<SearchResponse<T>> GetAggregates(RequestQuery<T> query);
        Task<SearchResponse<T>> Find(RequestQuery<T> query);

        // Custom
        FieldValue GetFieldValue(QueryFilter filter, object? value);
        FieldValue GetFieldValue(QueryFilter filter);
        Query CreateTermsQuery(QueryFilter filter);
        Query CreateTermQuery(QueryFilter filter);
        Query CreatExistsQuery(QueryFilter filter);
        Query CreateTerm(QueryFilter filter);
        Query[] GetQueries(RequestQuery request, ElasticCondition condition);
        QueryDescriptor<T> CreateQuery(RequestQuery request);
        QueryFilter Filter(string field, ElasticCondition cond, QueryOperator op);
        QueryFilter Filter(string field, object? value);
        QueryFilter Filter(string field, ElasticCondition cond, QueryOperator op, object? value);
        QueryFilter Filter(string field, ElasticCondition cond, QueryOperator op, List<object>? values);
    }
}

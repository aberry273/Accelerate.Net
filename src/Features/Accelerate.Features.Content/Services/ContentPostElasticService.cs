using Accelerate.Features.Content.Models.Data;
using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Options;

namespace Accelerate.Features.Content.Services
{
    public class ContentPostElasticService :  IContentPostElasticService
    {
        public IElasticService _service;
        private string _index = "contentpost_index";
        
        public ContentPostElasticService(IElasticService service)
        {
            _service = service;
        }  
        public async Task<IndexResponse> Index(ContentPostEntity doc)
        {
            //Create if not existing
            await _service.CreateIndex(_index);
            //Index
            return await _service.IndexDocument<ContentPostEntity>(doc, _index);
        }
        public async Task<SearchResponse<ContentPostEntity>> Find(RequestQuery<ContentPostEntity> query)
        {
            //Create if not existing
            await _service.CreateIndex(_index);
            //Search
            int take = query.ItemsPerPage > 0 ? query.ItemsPerPage : 10;
            int skip = take * query.Page;
            
            return await _service.SearchDocuments<ContentPostEntity>(_index,
                CreateQuery(query),
                skip,
                take);
        }

        private QueryDescriptor<ContentPostEntity> CreateQuery(RequestQuery<ContentPostEntity> request)
        {
            var descriptor =  new QueryDescriptor<ContentPostEntity>();
            descriptor.MatchAll();
            if (!string.IsNullOrEmpty(request?.Query?.Content))
                descriptor.Term(x => x.Content, request?.Query?.Content);
            return descriptor;
        }
    }
}

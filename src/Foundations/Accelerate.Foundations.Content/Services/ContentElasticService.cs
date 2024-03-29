using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;
using Microsoft.Extensions.Options;

namespace Accelerate.Foundations.Content.Services
{
    //Overwrite the core service for custom filtering
    public class ContentElasticService : ElasticService<ContentPostDocument>
    {

        public ContentElasticService(IOptions<ElasticConfiguration> options) : base(options)
        {
            this._indexName = "contentpost_index";
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

        private QueryDescriptor<ContentPostDocument> CreateQuery(RequestQuery<ContentPostDocument> request)
        {
            var descriptor =  new QueryDescriptor<ContentPostDocument>();
            descriptor.MatchAll();
            if (!string.IsNullOrEmpty(request?.Query?.Content))
                descriptor.Term(x => x.Content, request?.Query?.Content);
            return descriptor;
        }
    }
}

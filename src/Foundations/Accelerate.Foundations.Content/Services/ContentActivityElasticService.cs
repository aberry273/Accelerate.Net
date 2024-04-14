using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;
using Microsoft.Extensions.Options;

namespace Accelerate.Foundations.Content.Services
{
    public class ContentActivityElasticService :  ElasticService<ContentPostActivityDocument>
    {

        public ContentActivityElasticService(IOptions<ElasticConfiguration> options) : base(options)
        {
            this._indexName = "contentpostreview_index";
        }
        public override async Task<SearchResponse<ContentPostActivityDocument>> Find(RequestQuery<ContentPostActivityDocument> query)
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

        public override Task<SearchResponse<ContentPostActivityDocument>> GetAggregates(RequestQuery<ContentPostActivityDocument> query)
        {
            throw new NotImplementedException();
        }

        private QueryDescriptor<ContentPostActivityDocument> CreateQuery(RequestQuery<ContentPostActivityDocument> request)
        {
            var descriptor =  new QueryDescriptor<ContentPostActivityDocument>();
            descriptor.MatchAll();
          
            return descriptor;
        }
    }
}

using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;
using Microsoft.Extensions.Options;
using Accelerate.Foundations.Content.Models.Entities;

namespace Accelerate.Foundations.Content.Services
{
    public class ContentReviewElasticService :  ElasticService<ContentPostReviewDocument>
    {

        public ContentReviewElasticService(IOptions<ElasticConfiguration> options) : base(options)
        {
            this._indexName = "contentpostreview_index";
        }
        public override async Task<SearchResponse<ContentPostReviewDocument>> Find(RequestQuery<ContentPostReviewDocument> query)
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

        public override Task<SearchResponse<ContentPostReviewDocument>> GetAggregates(RequestQuery<ContentPostReviewDocument> query)
        {
            throw new NotImplementedException();
        }

        private QueryDescriptor<ContentPostReviewDocument> CreateQuery(RequestQuery<ContentPostReviewDocument> request)
        {
            var descriptor =  new QueryDescriptor<ContentPostReviewDocument>();
            descriptor.MatchAll();
          
            return descriptor;
        }
    }
}

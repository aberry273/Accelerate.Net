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
    public class ContentReviewElasticService :  ElasticService<ContentPostReviewEntity>
    {

        public ContentReviewElasticService(IOptions<ElasticConfiguration> options) : base(options)
        {
            this._indexName = "contentpostreview_index";
        }
        public override async Task<SearchResponse<ContentPostReviewEntity>> Find(RequestQuery<ContentPostReviewEntity> query)
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

        private QueryDescriptor<ContentPostReviewEntity> CreateQuery(RequestQuery<ContentPostReviewEntity> request)
        {
            var descriptor =  new QueryDescriptor<ContentPostReviewEntity>();
            descriptor.MatchAll();
          
            return descriptor;
        }
    }
}

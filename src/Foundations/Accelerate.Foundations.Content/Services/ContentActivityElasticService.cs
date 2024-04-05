using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;
using Microsoft.Extensions.Options;

namespace Accelerate.Foundations.Content.Services
{
    public class ContentActivityElasticService :  ElasticService<ContentPostActivityEntity>
    {

        public ContentActivityElasticService(IOptions<ElasticConfiguration> options) : base(options)
        {
            this._indexName = "contentpostreview_index";
        }
        public override async Task<SearchResponse<ContentPostActivityEntity>> Find(RequestQuery<ContentPostActivityEntity> query)
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

        public override Task<SearchResponse<ContentPostActivityEntity>> GetAggregates(RequestQuery<ContentPostActivityEntity> query)
        {
            throw new NotImplementedException();
        }

        private QueryDescriptor<ContentPostActivityEntity> CreateQuery(RequestQuery<ContentPostActivityEntity> request)
        {
            var descriptor =  new QueryDescriptor<ContentPostActivityEntity>();
            descriptor.MatchAll();
          
            return descriptor;
        }
    }
}

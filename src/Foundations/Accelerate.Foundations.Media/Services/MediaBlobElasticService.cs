
using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;
using Microsoft.Extensions.Options; 
using Accelerate.Foundations.Media.Models.Data;

namespace Accelerate.Foundations.Media.Services
{
    public class MediaBlobElasticService :  ElasticService<MediaBlobDocument>
    {

        public MediaBlobElasticService(IOptions<ElasticConfiguration> options) : base(options)
        {
            this._indexName = "mediablob_index";
        }
        public override async Task<SearchResponse<MediaBlobDocument>> Find(RequestQuery<MediaBlobDocument> query)
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

        public override Task<SearchResponse<MediaBlobDocument>> GetAggregates(RequestQuery<MediaBlobDocument> query)
        {
            throw new NotImplementedException();
        }

        private QueryDescriptor<MediaBlobDocument> CreateQuery(RequestQuery<MediaBlobDocument> request)
        {
            var descriptor =  new QueryDescriptor<MediaBlobDocument>();
            descriptor.MatchAll();
          
            return descriptor;
        }
    }
}

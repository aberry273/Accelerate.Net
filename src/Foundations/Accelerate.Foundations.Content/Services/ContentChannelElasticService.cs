using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Users.Models;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;
using Microsoft.Extensions.Options;
using Accelerate.Foundations.Content.Models.Entities;

namespace Accelerate.Foundations.Content.Services
{
    public class ContentChannelElasticService :  ElasticService<ContentChannelDocument>
    {

        public ContentChannelElasticService(IOptions<ElasticConfiguration> options, IOptions<ContentConfiguration> config) : base(options)
        {
            this._indexName = config.Value.ChannelIndexName;
        }
        public override async Task<SearchResponse<ContentChannelDocument>> Find(RequestQuery<ContentChannelDocument> query)
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

        public override Task<SearchResponse<ContentChannelDocument>> GetAggregates(RequestQuery<ContentChannelDocument> query, string sortByField = Constants.Fields.CreatedOn, SortOrder sortOrder = SortOrder.Asc)
        {
            throw new NotImplementedException();
        }

        private QueryDescriptor<ContentChannelDocument> CreateQuery(RequestQuery<ContentChannelDocument> request)
        {
            var descriptor =  new QueryDescriptor<ContentChannelDocument>();
            descriptor.MatchAll();
          
            return descriptor;
        }
    }
}

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
    public class ContentPostActionsElasticService :  ElasticService<ContentPostActionsDocument>
    {

        public ContentPostActionsElasticService(IOptions<ElasticConfiguration> options, IOptions<ContentConfiguration> config) : base(options)
        {
            this._indexName = config.Value.ActionsIndexName;
        }
        public override async Task<SearchResponse<ContentPostActionsDocument>> Find(RequestQuery<ContentPostActionsDocument> query)
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

        public override Task<SearchResponse<ContentPostActionsDocument>> GetAggregates(RequestQuery<ContentPostActionsDocument> query)
        {
            throw new NotImplementedException();
        }

        private QueryDescriptor<ContentPostActionsDocument> CreateQuery(RequestQuery<ContentPostActionsDocument> request)
        {
            var descriptor =  new QueryDescriptor<ContentPostActionsDocument>();
            descriptor.MatchAll();
          
            return descriptor;
        }
    }
}

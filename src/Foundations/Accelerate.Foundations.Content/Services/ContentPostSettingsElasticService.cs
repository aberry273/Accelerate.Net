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
    public class ContentPostSettingsElasticService : ElasticService<ContentPostSettingsDocument>
    {
        public ContentPostSettingsElasticService(IOptions<ElasticConfiguration> options) : base(options)
        {
            this._indexName = "contentpostAction_index";
        }
        public override async Task<SearchResponse<ContentPostSettingsDocument>> Find(RequestQuery<ContentPostSettingsDocument> query)
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

        public override Task<SearchResponse<ContentPostSettingsDocument>> GetAggregates(RequestQuery<ContentPostSettingsDocument> query, string sortByField = Constants.Fields.CreatedOn, SortOrder sortOrder = SortOrder.Asc)
        {
            throw new NotImplementedException();
        }

        private QueryDescriptor<ContentPostSettingsDocument> CreateQuery(RequestQuery<ContentPostSettingsDocument> request)
        {
            var descriptor = new QueryDescriptor<ContentPostSettingsDocument>();
            descriptor.MatchAll();

            return descriptor;
        }
    }
}

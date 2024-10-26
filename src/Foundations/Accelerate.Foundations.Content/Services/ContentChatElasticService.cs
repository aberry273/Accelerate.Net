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
    public class ContentChatElasticService :  ElasticService<ContentChatDocument>
    {

        public ContentChatElasticService(IOptions<ElasticConfiguration> options, IOptions<ContentConfiguration> config) : base(options)
        {
            this._indexName = config.Value.ChatIndexName;
        }
        public override async Task<SearchResponse<ContentChatDocument>> Find(RequestQuery<ContentChatDocument> query)
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

        public override Task<SearchResponse<ContentChatDocument>> GetAggregates(RequestQuery<ContentChatDocument> query, string sortByField = Constants.Fields.CreatedOn, SortOrder sortOrder = SortOrder.Asc)
        {
            throw new NotImplementedException();
        }

        private QueryDescriptor<ContentChatDocument> CreateQuery(RequestQuery<ContentChatDocument> request)
        {
            var descriptor =  new QueryDescriptor<ContentChatDocument>();
            descriptor.MatchAll();
          
            return descriptor;
        }
    }
}

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
    public class ContentPostLabelElasticService : ElasticService<ContentPostLabelDocument>
    {

        public ContentPostLabelElasticService(
            IOptions<ElasticConfiguration> options,
            IOptions<ContentConfiguration> config) : base(options)
        {
            this._indexName = config.Value.ActivitiesIndexName;
        }
        public override async Task<SearchResponse<ContentPostLabelDocument>> Find(RequestQuery<ContentPostLabelDocument> query)
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

        public override Task<SearchResponse<ContentPostLabelDocument>> GetAggregates(RequestQuery<ContentPostLabelDocument> query)
        {
            throw new NotImplementedException();
        }

        private QueryDescriptor<ContentPostLabelDocument> CreateQuery(RequestQuery<ContentPostLabelDocument> request)
        {
            var descriptor =  new QueryDescriptor<ContentPostLabelDocument>();
            descriptor.MatchAll();
          
            return descriptor;
        }

        public QueryDescriptor<ContentPostLabelDocument> BuildUserSearchQuery(Guid userId)
        {
            var Query = new RequestQuery();

            Query.Filters.Add(Filter(Constants.Fields.UserId, ElasticCondition.Filter, userId));

            return this.CreateQuery(Query);
        }

        public async Task<SearchResponse<ContentPostLabelDocument>> SearchUserDocuments(Guid userId, int page = 0, int itemsPerPage = 10)
        {
            var elasticQuery = BuildUserSearchQuery(userId);
            //TODO: remove

            int take = itemsPerPage > 0 ? itemsPerPage : ItemsPerPage;
            if (take > MaxQueryable) take = MaxQueryable;
            int skip = take * page;

            var model = new ContentPostLabelDocument();
            var results = await Search(elasticQuery, skip, take);
            return results;
        }
    }
}

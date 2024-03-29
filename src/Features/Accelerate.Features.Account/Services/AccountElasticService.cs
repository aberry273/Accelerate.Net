
using Accelerate.Features.Account.Models;
using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;
using Microsoft.Extensions.Options;

namespace Accelerate.Features.Account.Services
{
    //Overwrite the core service for custom filtering
    public class AccountElasticService : ElasticService<AccountUserDocument>
    {

        public AccountElasticService(IOptions<ElasticConfiguration> options) : base(options)
        {
            this._indexName = "accountuser_index";
        }
        public override async Task<SearchResponse<AccountUserDocument>> Find(RequestQuery<AccountUserDocument> query)
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

        private QueryDescriptor<AccountUserDocument> CreateQuery(RequestQuery<AccountUserDocument> request)
        {
            var descriptor =  new QueryDescriptor<AccountUserDocument>();
            descriptor.MatchAll();
            if (!string.IsNullOrEmpty(request?.Query?.Username))
                descriptor.Term(x => x.Username, request?.Query?.Username);
            return descriptor;
        }
    }
}

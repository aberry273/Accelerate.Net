using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Aggregations;
using Elastic.Clients.Elasticsearch.Core.MSearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks; 

namespace Accelerate.Foundations.Integrations.Elastic.Services
{
    public class BaseElasticService<T> : ElasticService<T>
    {
        public BaseElasticService(IOptions<ElasticConfiguration> options, string indexName) : base(options, indexName)
        { 
        }
        public override async Task<SearchResponse<T>> Find(RequestQuery<T> query)
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

        public override Task<SearchResponse<T>> GetAggregates(RequestQuery<T> query, string sortByField = Constants.Fields.CreatedOn, SortOrder sortOrder = SortOrder.Asc)
        {
            throw new NotImplementedException();
        }

        private QueryDescriptor<T> CreateQuery(RequestQuery<T> request)
        {
            var descriptor = new QueryDescriptor<T>();
            descriptor.MatchAll();

            return descriptor;
        }
    }
}

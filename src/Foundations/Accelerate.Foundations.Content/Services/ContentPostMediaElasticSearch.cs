using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;
using Microsoft.Extensions.Options;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Elastic.Clients.Elasticsearch.Mapping;
using Elastic.Clients.Elasticsearch.Aggregations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Accelerate.Foundations.Common.Extensions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Accelerate.Foundations.Content.Services
{
    //Overwrite the core service for custom filtering
    public class ContentPostMediaElasticService : ElasticService<ContentPostMediaDocument>
    {

        public ContentPostMediaElasticService(IOptions<ElasticConfiguration> options) : base(options)
        {
            this._indexName = "contentpostmedia_index";
            _settings = new IndexSettings()
            {
                //NumberOfReplicas = 0,
            };
            this._mapping = CreateContentPostMapping();
        }

        private TypeMapping CreateContentPostMapping()
        {
            var mapping = new TypeMapping();
            return mapping;
        } 
        public override async Task<SearchResponse<ContentPostMediaDocument>> GetAggregates(RequestQuery<ContentPostMediaDocument> request)
        {
            return await base.GetAggregates(request);
        }
        public override async Task<SearchResponse<ContentPostMediaDocument>> Find(RequestQuery<ContentPostMediaDocument> query)
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
        public async Task<DeleteIndexResponse> DeleteIndex()
        {
            return await base.DeleteIndex();
        }

        private QueryDescriptor<ContentPostMediaDocument> CreateQuery(RequestQuery<ContentPostMediaDocument> request)
        {
            var descriptor = new QueryDescriptor<ContentPostMediaDocument>();
            descriptor.MatchAll();
           
            return descriptor;
        } 

    }
}

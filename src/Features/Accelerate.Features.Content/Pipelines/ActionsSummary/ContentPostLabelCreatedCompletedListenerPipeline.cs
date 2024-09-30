using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Account.Services;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.MSearch;
using Elastic.Clients.Elasticsearch.Ingest;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Accelerate.Foundations.Content.Hydrators;
using System;

namespace Accelerate.Features.Content.Pipelines.ActionsSummary
{
    public class ContentPostLabelCreatedCompletedListenerPipeline : DataCreateCompletedEventPipeline<ContentPostLabelEntity>
    {
        IHubContext<BaseHub<ContentPostActionsSummaryDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsSummaryDocument>>> _messageHub;
        IEntityService<ContentPostQuoteEntity> _entityQuoteService;
        IElasticService<ContentPostActionsSummaryDocument> _elasticService;
        IElasticService<ContentPostDocument> _elasticPostService;
        IEntityService<ContentPostActionsSummaryEntity> _entityService;
        public ContentPostLabelCreatedCompletedListenerPipeline(
            IEntityService<ContentPostActionsSummaryEntity> entityService, 
            IEntityService<ContentPostQuoteEntity> entityQuoteService,
            IElasticService<ContentPostDocument> elasticPostService,
            IElasticService<ContentPostActionsSummaryDocument> elasticService,
            IHubContext<BaseHub<ContentPostActionsSummaryDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsSummaryDocument>>> messageHub)
        {
            _entityService = entityService;
            _entityQuoteService = entityQuoteService;
            _elasticPostService = elasticPostService;
            _elasticService = elasticService;
            _messageHub = messageHub;

            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostLabelEntity>>()
            {
                //UpdatePostIndex
            };
        }
         

        public async Task UpdatePostIndex(IPipelineArgs<ContentPostLabelEntity> args)
        {
            var postResult = await _elasticPostService.GetDocument<ContentPostDocument>(args.Value.ContentPostId.ToString());
            var post = postResult.Source;
            //if (post.Taxonomy == null) 
            {
                post.Taxonomy = new ContentPostTaxonomySubdocument()
                {
                    Labels = new List<string>()
                };
            }
            if (post.Taxonomy.Labels == null)
            {
                //post.Taxonomy.Labels = new List<string>();
            }
            post.Taxonomy.Labels.Add(args.Value.Label);
        
            await _elasticPostService.UpdateDocument<ContentPostDocument>(post, args.Value.ContentPostId.ToString());
        }
    }
}

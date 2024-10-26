using Accelerate.Foundations.Users.Models;
using Accelerate.Foundations.Users.Services;
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
    public class ContentPostActionsSummaryCreatedPipeline : DataCreateEventPipeline<ContentPostActionsSummaryEntity>
    {
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHubPosts;
        IHubContext<BaseHub<ContentPostActionsSummaryDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsSummaryDocument>>> _messageHub;
        IEntityService<ContentPostActivityEntity> _entityService;
        IElasticService<ContentPostActionsSummaryDocument> _elasticService;
        IElasticService<ContentPostDocument> _elasticPostService;
        public ContentPostActionsSummaryCreatedPipeline(
            IElasticService<ContentPostActionsSummaryDocument> elasticService,
            IElasticService<ContentPostDocument> elasticPostService,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHubPosts,
            IEntityService<ContentPostActivityEntity> entityService,
            IHubContext<BaseHub<ContentPostActionsSummaryDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsSummaryDocument>>> messageHub)
        {
            _elasticService = elasticService;
            _elasticPostService = elasticPostService;
            _messageHub = messageHub;
            _entityService = entityService;
            _messageHubPosts = messageHubPosts;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostActionsSummaryEntity>>()
            {
                //IndexDocument,

                //UpdatePostIndex,
            };
            _processors = new List<PipelineProcessor<ContentPostActionsSummaryEntity>>()
            {
            };
        }
        // ASYNC PROCESSORS
        public async Task IndexDocument(IPipelineArgs<ContentPostActionsSummaryEntity> args)
        {
            var indexModel = new ContentPostActionsSummaryDocument();
            args.Value.Hydrate(indexModel);
            await _elasticService.Index(indexModel);
             
            await ContentPostActionsSummaryUtilities.SendWebsocketActionsSummaryUpdate(_messageHub, indexModel, "Create ActionsSummary successful", DataRequestCompleteType.Created);
        }
        /*
        public async Task UpdatePostIndex(IPipelineArgs<ContentPostActionsSummaryEntity> args)
        {
            // fetch Actions
            var ActionsDoc = ContentPostActionsSummaryUtilities.GetActivities(_entityService, args);
            var fetchResponse = await _elasticPostService.GetDocument<ContentPostDocument>(args.Value.ContentPostId.ToString());
            var contentPostDocument = fetchResponse.Source;
            contentPostDocument.ActionsTotals = ActionsDoc;
            contentPostDocument.UpdatedOn = DateTime.Now;
            await _elasticPostService.UpdateDocument(contentPostDocument, args.Value?.ContentPostId.ToString());

            // Send websocket request
            await ContentPostActionsSummaryUtilities.SendWebsocketPostUpdate(_messageHubPosts, args.Value?.UserId.ToString(), contentPostDocument, DataRequestCompleteType.Updated);
        } 
        */

        // SYNC PROCESSORS
    }
}

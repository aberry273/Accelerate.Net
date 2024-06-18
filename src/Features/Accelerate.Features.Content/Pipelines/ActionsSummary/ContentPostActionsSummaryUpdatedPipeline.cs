﻿using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Account.Services;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.MSearch;
using Elastic.Clients.Elasticsearch.Ingest;
using Elastic.Clients.Elasticsearch.QueryDsl;
using MassTransit.DependencyInjection;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Accelerate.Features.Content.EventBus;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Content.Hydrators;

namespace Accelerate.Features.Content.Pipelines.ActionsSummary
{
    public class ContentPostActionsSummaryUpdatedPipeline : DataUpdateEventPipeline<ContentPostActionsSummaryEntity>
    {
        readonly Bind<IContentPostBus, IPublishEndpoint> _publishEndpoint;
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHubPosts;
        IHubContext<BaseHub<ContentPostActionsSummaryDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsSummaryDocument>>> _messageHub;
        IElasticService<ContentPostDocument> _elasticPostService;
        IEntityService<ContentPostActivityEntity> _entityService;
        IElasticService<ContentPostActionsSummaryDocument> _elasticService;
        public ContentPostActionsSummaryUpdatedPipeline(
            Bind<IContentPostBus, IPublishEndpoint> publishEndpoint,
            IElasticService<ContentPostActionsSummaryDocument> elasticService,
            IElasticService<ContentPostDocument> elasticPostService,
            IEntityService<ContentPostActivityEntity> entityService,
            IHubContext<BaseHub<ContentPostActionsSummaryDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsSummaryDocument>>> messageHub,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHubPosts)
        {
            _messageHub = messageHub;
            _messageHubPosts = messageHubPosts;
            _elasticService = elasticService;
            _elasticPostService = elasticPostService;
            _entityService = entityService;
            _publishEndpoint = publishEndpoint;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostActionsSummaryEntity>>()
            {
                IndexDocument,
                UpdatePostIndex,
               // PublishPostUpdatedMessage
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
            var result = await _elasticService.UpdateOrCreateDocument(indexModel, args.Value.Id.ToString());
            // Run directly in same function as Actions are simple types and will not be extended via pipelines
            if(result.IsValidResponse)
            {
                var docArgs = new PipelineArgs<ContentPostActionsSummaryDocument>()
                {
                    Value = indexModel
                };
                await ContentPostActionsSummaryUtilities.SendWebsocketActionsSummaryUpdate(_messageHub, docArgs, "Update Summary Action successful", DataRequestCompleteType.Updated);
            }
            args.Params["IndexedModel"] = indexModel; 
        }
        /// <summary>
        /// TODO: REFACTOR THIS ONCE POC READY
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
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
            await ContentPostActionsSummaryUtilities.SendWebsocketPostUpdate(_messageHubPosts ,args.Value?.UserId.ToString(), contentPostDocument, DataRequestCompleteType.Updated);
        }
    }
}
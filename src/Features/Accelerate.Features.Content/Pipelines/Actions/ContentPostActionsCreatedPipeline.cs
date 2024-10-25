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
using Accelerate.Foundations.Content.EventBus;
using MassTransit.DependencyInjection;
using MassTransit;
using Accelerate.Foundations.EventPipelines.Models.Contracts;

namespace Accelerate.Features.Content.Pipelines.Actions
{
    public class ContentPostActionsCreatedPipeline : DataCreateEventPipeline<ContentPostActionsEntity>
    {
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHubPosts;
        IHubContext<BaseHub<ContentPostActionsDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsDocument>>> _messageHub;
        IEntityService<ContentPostActionsEntity> _entityService;
        IElasticService<ContentPostActionsDocument> _elasticService;
        IElasticService<ContentPostDocument> _elasticPostService;
        public ContentPostActionsCreatedPipeline(
            IElasticService<ContentPostActionsDocument> elasticService,
            IElasticService<ContentPostDocument> elasticPostService,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHubPosts,
            IEntityService<ContentPostActionsEntity> entityService,
            IHubContext<BaseHub<ContentPostActionsDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsDocument>>> messageHub)
        {
            _elasticService = elasticService;
            _elasticPostService = elasticPostService;
            _messageHub = messageHub;
            _entityService = entityService;
            _messageHubPosts = messageHubPosts;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostActionsEntity>>()
            {
                IndexDocument,
                //UpdatePostIndex,
            };
            _processors = new List<PipelineProcessor<ContentPostActionsEntity>>()
            {
            };
        }
        // ASYNC PROCESSORS
        public async Task IndexDocument(IPipelineArgs<ContentPostActionsEntity> args)
        {
            var indexModel = new ContentPostActionsDocument();
            args.Value.Hydrate(indexModel);
            await _elasticService.Index(indexModel);

            var docArgs = new PipelineArgs<ContentPostActionsDocument>()
            {
                Value = indexModel
            };
            await ContentPostActionUtilities.SendWebsocketActionUpdate(_messageHub, docArgs, "Create Action successful", DataRequestCompleteType.Created);
        }
        public async Task UpdatePostIndex(IPipelineArgs<ContentPostActionsEntity> args)
        { 
            // fetch Actions
            var ActionsDoc = ContentPostActionUtilities.GetActions(_entityService, args);
            var fetchResponse = await _elasticPostService.GetDocument<ContentPostDocument>(args.Value.ContentPostId.ToString());
            var contentPostDocument = fetchResponse.Source;
            contentPostDocument.Id = args.Value.ContentPostId;
            contentPostDocument.ActionsTotals = ActionsDoc;
            contentPostDocument.UpdatedOn = DateTime.Now;
            await _elasticPostService.UpdateOrCreateDocument(contentPostDocument, args.Value?.ContentPostId.ToString());

            // Send websocket request
            await ContentPostUtilities.SendWebsocketPostUpdate(_messageHubPosts, args.Value?.UserId.ToString(), contentPostDocument, DataRequestCompleteType.Updated);
        }
          

        // SYNC PROCESSORS
    }
}

﻿using Accelerate.Foundations.Account.Models;
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
using Accelerate.Foundations.EventPipelines.Services;

namespace Accelerate.Features.Content.Pipelines.Quotes
{
    public class ContentPostQuoteCreatedPipeline : DataCreateEventPipeline<ContentPostQuoteEntity>
    {
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHubPosts;
        IHubContext<BaseHub<ContentPostQuoteDocument>, IBaseHubClient<WebsocketMessage<ContentPostQuoteDocument>>> _messageHub;
        IEntityService<ContentPostQuoteEntity> _entityService;
        IElasticService<ContentPostQuoteDocument> _elasticService;
        IElasticService<ContentPostDocument> _elasticPostService;
        IEntityPipelineService<ContentPostActivityEntity, IContentPostActivityBus> _pipelineActivityService;

        public ContentPostQuoteCreatedPipeline(
            IElasticService<ContentPostQuoteDocument> elasticService,
            IElasticService<ContentPostDocument> elasticPostService,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHubPosts,
            IEntityService<ContentPostQuoteEntity> entityService,
            IHubContext<BaseHub<ContentPostQuoteDocument>, IBaseHubClient<WebsocketMessage<ContentPostQuoteDocument>>> messageHub,
            IEntityPipelineService<ContentPostActivityEntity, IContentPostActivityBus> pipelineActivityService)
        {
            _elasticService = elasticService;
            _elasticPostService = elasticPostService;
            _messageHub = messageHub;
            _entityService = entityService;
            _messageHubPosts = messageHubPosts;
            _pipelineActivityService = pipelineActivityService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostQuoteEntity>>()
            {
                IndexDocument,
                CreateQuoteActivity
                //UpdatePostIndex,
            };
            _processors = new List<PipelineProcessor<ContentPostQuoteEntity>>()
            {
            };
        }
        // ASYNC PROCESSORS
        public async Task IndexDocument(IPipelineArgs<ContentPostQuoteEntity> args)
        {
            var indexModel = new ContentPostQuoteDocument();
            args.Value.Hydrate(indexModel);
            await _elasticService.Index(indexModel);

            var docArgs = new PipelineArgs<ContentPostQuoteDocument>()
            {
                Value = indexModel
            };
            await ContentPostParentUtilities.SendWebsocketQuoteUpdate(_messageHub, docArgs, "Create Quote successful", DataRequestCompleteType.Created);
        }
        private async Task CreateQuoteActivity(IPipelineArgs<ContentPostQuoteEntity> args)
        {
            var entity = new ContentPostActivityEntity()
            {
                Type = ContentPostActivityTypes.Created,
                UserId = args.Value.UserId,
                Message = "You were quoted in a post!",
                Url = $"/Threads/{args.Value.ContentPostId}"
            };
            await _pipelineActivityService.Create(entity);
        }
        public async Task UpdatePostIndex(IPipelineArgs<ContentPostQuoteEntity> args)
        {
            // fetch Actions
            var ActionsDoc = ContentPostParentUtilities.GetTotalQuotes(_entityService, args);
            var fetchResponse = await _elasticPostService.GetDocument<ContentPostDocument>(args.Value.ContentPostId.ToString());
            var contentPostDocument = fetchResponse.Source;

            contentPostDocument.UpdatedOn = DateTime.Now;
            await _elasticPostService.UpdateDocument(contentPostDocument, args.Value?.ContentPostId.ToString());

            // Send websocket request
            await ContentPostParentUtilities.SendWebsocketPostUpdate(_messageHubPosts, args.Value?.UserId.ToString(), contentPostDocument, DataRequestCompleteType.Updated);
        } 
          

        // SYNC PROCESSORS
    }
}

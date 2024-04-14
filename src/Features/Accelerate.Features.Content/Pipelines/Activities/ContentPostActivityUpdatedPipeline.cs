using Accelerate.Features.Content.Services;
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

namespace Accelerate.Features.Content.Pipelines.Activities
{
    public class ContentPostActivityUpdatedPipeline : DataUpdateEventPipeline<ContentPostActivityEntity>
    {
        readonly Bind<IContentPostBus, IPublishEndpoint> _publishEndpoint;
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHubPosts;
        IHubContext<BaseHub<ContentPostActivityDocument>, IBaseHubClient<WebsocketMessage<ContentPostActivityDocument>>> _messageHub;
        IElasticService<ContentPostDocument> _elasticPostService;
        IEntityService<ContentPostActivityEntity> _entityService;
        IElasticService<ContentPostActivityDocument> _elasticService;
        public ContentPostActivityUpdatedPipeline(
            Bind<IContentPostBus, IPublishEndpoint> publishEndpoint,
            IElasticService<ContentPostActivityDocument> elasticService,
            IElasticService<ContentPostDocument> elasticPostService,
            IEntityService<ContentPostActivityEntity> entityService,
            IHubContext<BaseHub<ContentPostActivityDocument>, IBaseHubClient<WebsocketMessage<ContentPostActivityDocument>>> messageHub,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHubPosts)
        {
            _messageHub = messageHub;
            _messageHubPosts = messageHubPosts;
            _elasticService = elasticService;
            _elasticPostService = elasticPostService;
            _entityService = entityService;
            _publishEndpoint = publishEndpoint;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostActivityEntity>>()
            {
                IndexDocument,
                UpdatePostIndex,
               // PublishPostUpdatedMessage
            };
            _processors = new List<PipelineProcessor<ContentPostActivityEntity>>()
            {
            };
        }
        // ASYNC PROCESSORS
        public async Task IndexDocument(IPipelineArgs<ContentPostActivityEntity> args)
        {
            var indexModel = new ContentPostActivityDocument();
            args.Value.Hydrate(indexModel);
            var result = await _elasticService.UpdateOrCreateDocument(indexModel, args.Value.Id.ToString());
            // Run directly in same function as reviews are simple types and will not be extended via pipelines
            if(result.IsValidResponse)
            {
                var docArgs = new PipelineArgs<ContentPostActivityDocument>()
                {
                    Value = indexModel
                };
                await ContentPostActivityUtilities.SendWebsocketActivityUpdate(_messageHub, docArgs, "Update review successful", DataRequestCompleteType.Updated);
            }
            args.Params["IndexedModel"] = indexModel; 
        }
        /// <summary>
        /// TODO: REFACTOR THIS ONCE POC READY
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task UpdatePostIndex(IPipelineArgs<ContentPostActivityEntity> args)
        {
            // fetch reviews
            var reviewsDoc = ContentPostActivityUtilities.GetActivities(_entityService, args);
            var fetchResponse = await _elasticPostService.GetDocument<ContentPostDocument>(args.Value.ContentPostId.ToString());
            var contentPostDocument = fetchResponse.Source; 
            contentPostDocument.Reviews = reviewsDoc;
            contentPostDocument.UpdatedOn = DateTime.Now;
            await _elasticPostService.UpdateDocument(contentPostDocument, args.Value?.ContentPostId.ToString());

            // Send websocket request
            await ContentPostActivityUtilities.SendWebsocketPostUpdate(_messageHubPosts ,args.Value?.UserId.ToString(), contentPostDocument, DataRequestCompleteType.Updated);
        }
    }
}

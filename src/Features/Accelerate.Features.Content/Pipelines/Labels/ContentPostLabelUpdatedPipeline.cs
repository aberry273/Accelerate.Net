using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Users.Models;
using Accelerate.Foundations.Users.Services;
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
using Accelerate.Foundations.Content.EventBus;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Content.Hydrators;
using Accelerate.Foundations.EventPipelines.Services;

namespace Accelerate.Features.Content.Pipelines.Labels
{
    public class ContentPostLabelUpdatedPipeline : DataUpdateEventPipeline<ContentPostLabelEntity>
    {
        readonly Bind<IContentPostBus, IPublishEndpoint> _publishEndpoint;
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHubPosts;
        IHubContext<BaseHub<ContentPostLabelDocument>, IBaseHubClient<WebsocketMessage<ContentPostLabelDocument>>> _messageHub;
        IElasticService<ContentPostDocument> _elasticPostService;
        IEntityService<ContentPostLabelEntity> _entityService;
        IElasticService<ContentPostLabelDocument> _elasticService;
        IEntityPipelineService<ContentPostActivityEntity, IContentPostActivityBus> _pipelineActivityService;

        public ContentPostLabelUpdatedPipeline(
            Bind<IContentPostBus, IPublishEndpoint> publishEndpoint,
            IElasticService<ContentPostLabelDocument> elasticService,
            IElasticService<ContentPostDocument> elasticPostService,
            IEntityService<ContentPostLabelEntity> entityService,
            IEntityPipelineService<ContentPostActivityEntity, IContentPostActivityBus> pipelineActivityService,
            IHubContext<BaseHub<ContentPostLabelDocument>, IBaseHubClient<WebsocketMessage<ContentPostLabelDocument>>> messageHub,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHubPosts)
        {
            _messageHub = messageHub;
            _messageHubPosts = messageHubPosts;
            //_elasticService = elasticService;
            _elasticPostService = elasticPostService;
            _entityService = entityService;
            _publishEndpoint = publishEndpoint;
            _pipelineActivityService = pipelineActivityService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostLabelEntity>>()
            {
                IndexDocument, 
               // PublishPostUpdatedMessage
            };
            _processors = new List<PipelineProcessor<ContentPostLabelEntity>>()
            {
            };
        }
        // ASYNC PROCESSORS
        public async Task IndexDocument(IPipelineArgs<ContentPostLabelEntity> args)
        {
            var indexModel = new ContentPostLabelDocument();
            args.Value.Hydrate(indexModel);
            //var result = await _elasticService.UpdateOrCreateDocument(indexModel, args.Value.Id.ToString());
            // Run directly in same function as Label are simple types and will not be extended via pipelines
            /*
            if(result.IsValidResponse)
            {
                var docArgs = new PipelineArgs<ContentPostLabelDocument>()
                {
                    Value = indexModel
                };
                await ContentPostLabelUtilities.SendWebsocketLabelUpdate(_messageHub, docArgs, "Update Action successful", DataRequestCompleteType.Updated);
            }
            */
        } 
    }
}

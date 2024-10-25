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
using Accelerate.Foundations.Content.EventBus;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Content.Hydrators;
using Accelerate.Foundations.EventPipelines.Services;

namespace Accelerate.Features.Content.Pipelines.Mentions
{
    public class ContentPostMentionUpdatedPipeline : DataUpdateEventPipeline<ContentPostMentionEntity>
    {
        readonly Bind<IContentPostBus, IPublishEndpoint> _publishEndpoint;
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHubPosts;
        IHubContext<BaseHub<ContentPostMentionDocument>, IBaseHubClient<WebsocketMessage<ContentPostMentionDocument>>> _messageHub;
        IElasticService<ContentPostDocument> _elasticPostService;
        IEntityService<ContentPostMentionEntity> _entityService;
        //IElasticService<ContentPostMentionDocument> _elasticService;
        IEntityPipelineService<ContentPostActivityEntity, IContentPostActivityBus> _pipelineActivityService;

        public ContentPostMentionUpdatedPipeline(
            Bind<IContentPostBus, IPublishEndpoint> publishEndpoint,
            //IElasticService<ContentPostMentionDocument> elasticService,
            IElasticService<ContentPostDocument> elasticPostService,
            IEntityService<ContentPostMentionEntity> entityService,
            IEntityPipelineService<ContentPostActivityEntity, IContentPostActivityBus> pipelineActivityService,
            IHubContext<BaseHub<ContentPostMentionDocument>, IBaseHubClient<WebsocketMessage<ContentPostMentionDocument>>> messageHub,
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
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostMentionEntity>>()
            {
                //IndexDocument, 
               // PublishPostUpdatedMessage
            };
            _processors = new List<PipelineProcessor<ContentPostMentionEntity>>()
            {
            };
        }
        // ASYNC PROCESSORS
        public async Task IndexDocument(IPipelineArgs<ContentPostMentionEntity> args)
        {
            var indexModel = new ContentPostMentionDocument();
            args.Value.Hydrate(indexModel);
            //var result = await _elasticService.UpdateOrCreateDocument(indexModel, args.Value.Id.ToString());
            // Run directly in same function as Mention are simple types and will not be extended via pipelines
            /*
            if(result.IsValidResponse)
            {
                var docArgs = new PipelineArgs<ContentPostMentionDocument>()
                {
                    Value = indexModel
                };
                await ContentPostMentionUtilities.SendWebsocketMentionUpdate(_messageHub, docArgs, "Update Action successful", DataRequestCompleteType.Updated);
            }
            */
        } 
    }
}

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
using Accelerate.Foundations.EventPipelines.Services;

namespace Accelerate.Features.Content.Pipelines.Mentions
{
    public class ContentPostMentionCreatedPipeline : DataCreateEventPipeline<ContentPostMentionEntity>
    {
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHubPosts;
        IHubContext<BaseHub<ContentPostMentionDocument>, IBaseHubClient<WebsocketMessage<ContentPostMentionDocument>>> _messageHub;
        IEntityService<ContentPostMentionEntity> _entityService;
        //IElasticService<ContentPostMentionDocument> _elasticService;
        IElasticService<ContentPostDocument> _elasticPostService;
        IEntityPipelineService<ContentPostActivityEntity, IContentPostActivityBus> _pipelineActivityService;
        public ContentPostMentionCreatedPipeline(
            //IElasticService<ContentPostMentionDocument> elasticService,
            IElasticService<ContentPostDocument> elasticPostService,
            IEntityPipelineService<ContentPostActivityEntity, IContentPostActivityBus> pipelineActivityService,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHubPosts,
            IEntityService<ContentPostMentionEntity> entityService,
            IHubContext<BaseHub<ContentPostMentionDocument>, IBaseHubClient<WebsocketMessage<ContentPostMentionDocument>>> messageHub)
        {
            //_elasticService = elasticService;
            _elasticPostService = elasticPostService;
            _pipelineActivityService = pipelineActivityService;
            _messageHub = messageHub;
            _entityService = entityService;
            _messageHubPosts = messageHubPosts;
            _pipelineActivityService = pipelineActivityService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostMentionEntity>>()
            {
                //IndexDocument,
                CreatePostActivity
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
            //await _elasticService.Index(indexModel);

            var docArgs = new PipelineArgs<ContentPostMentionDocument>()
            {
                Value = indexModel
            };
            await ContentPostMentionUtilities.SendWebsocketMentionUpdate(_messageHub, docArgs, "Create Action successful", DataRequestCompleteType.Created);
        }
        private async Task CreatePostActivity(IPipelineArgs<ContentPostMentionEntity> args)
        {
            var entity = new ContentPostActivityEntity()
            {
                Type = ContentPostActivityTypes.Created,
                UserId = args.Value.UserId,
                Message = "You were mentioned in a post",
                Url = $"/Threads/{args.Value.ContentPostId}"
            };
            await _pipelineActivityService.Create(entity);
        }

        // SYNC PROCESSORS
    }
}

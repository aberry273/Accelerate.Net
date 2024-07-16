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

namespace Accelerate.Features.Content.Pipelines.Labels
{
    public class ContentPostLabelCreatedPipeline : DataCreateEventPipeline<ContentPostLabelEntity>
    {
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHubPosts;
        IHubContext<BaseHub<ContentPostLabelDocument>, IBaseHubClient<WebsocketMessage<ContentPostLabelDocument>>> _messageHub;
        IEntityService<ContentPostLabelEntity> _entityService;
        IElasticService<ContentPostLabelDocument> _elasticService;
        IElasticService<ContentPostDocument> _elasticPostService;
        IEntityPipelineService<ContentPostActivityEntity, IContentPostActivityBus> _pipelineActivityService;
        public ContentPostLabelCreatedPipeline(
            IElasticService<ContentPostLabelDocument> elasticService,
            IElasticService<ContentPostDocument> elasticPostService,
            IEntityPipelineService<ContentPostActivityEntity, IContentPostActivityBus> pipelineActivityService,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHubPosts,
            IEntityService<ContentPostLabelEntity> entityService,
            IHubContext<BaseHub<ContentPostLabelDocument>, IBaseHubClient<WebsocketMessage<ContentPostLabelDocument>>> messageHub)
        {
            //_elasticService = elasticService;
            _elasticPostService = elasticPostService;
            _pipelineActivityService = pipelineActivityService;
            _messageHub = messageHub;
            _entityService = entityService;
            _messageHubPosts = messageHubPosts;
            _pipelineActivityService = pipelineActivityService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostLabelEntity>>()
            {
                //IndexDocument,
                CreatePostActivity
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
            await _elasticService.Index(indexModel);
            /*
            var docArgs = new PipelineArgs<ContentPostLabelDocument>()
            {
                Value = indexModel
            };
            await ContentPostLabelUtilities.SendWebsocketLabelUpdate(_messageHub, docArgs, "Create Label successful", DataRequestCompleteType.Created);
            */
        }
        private async Task CreatePostActivity(IPipelineArgs<ContentPostLabelEntity> args)
        {
            var entity = new ContentPostActivityEntity()
            {
                Type = ContentPostActivityTypes.Created,
                UserId = args.Value.UserId,
                Message = $"Your post was labeled as '{args.Value.Label}'",
                Url = $"/Threads/{args.Value.ContentPostId}"
            };
            await _pipelineActivityService.Create(entity);
        }

        // SYNC PROCESSORS
    }
}

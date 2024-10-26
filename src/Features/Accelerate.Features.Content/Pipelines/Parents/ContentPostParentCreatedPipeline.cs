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
using Accelerate.Foundations.Content.EventBus;
using Accelerate.Foundations.EventPipelines.Services;

namespace Accelerate.Features.Content.Pipelines.Parents
{
    public class ContentPostParentCreatedPipeline : DataCreateEventPipeline<ContentPostParentEntity>
    {
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHubPosts;
        //IHubContext<BaseHub<ContentPostParentDocument>, IBaseHubClient<WebsocketMessage<ContentPostParentDocument>>> _messageHub;
        IEntityService<ContentPostParentEntity> _entityService;
        //IElasticService<ContentPostParentDocument> _elasticService;
        IElasticService<ContentPostDocument> _elasticPostService;
        IEntityPipelineService<ContentPostActivityEntity, IContentPostActivityBus> _pipelineActivityService;

        public ContentPostParentCreatedPipeline(
            //IElasticService<ContentPostParentDocument> elasticService,
            IElasticService<ContentPostDocument> elasticPostService,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHubPosts,
            IEntityService<ContentPostParentEntity> entityService,
            //IHubContext<BaseHub<ContentPostParentDocument>, IBaseHubClient<WebsocketMessage<ContentPostParentDocument>>> messageHub,
            IEntityPipelineService<ContentPostActivityEntity, IContentPostActivityBus> pipelineActivityService)
        {
            //_elasticService = elasticService;
            _elasticPostService = elasticPostService;
            //_messageHub = messageHub;
            _entityService = entityService;
            _messageHubPosts = messageHubPosts;
            _pipelineActivityService = pipelineActivityService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostParentEntity>>()
            {
                //IndexDocument,
                CreateParentActivity
                //UpdatePostIndex,
            };
            _processors = new List<PipelineProcessor<ContentPostParentEntity>>()
            {
            };
        }
        // ASYNC PROCESSORS
        /*
        public async Task IndexDocument(IPipelineArgs<ContentPostParentEntity> args)
        {
            var indexModel = new ContentPostParentDocument();
            args.Value.Hydrate(indexModel);
            await _elasticService.Index(indexModel);

            var docArgs = new PipelineArgs<ContentPostParentDocument>()
            {
                Value = indexModel
            };
            await ContentPostParentUtilities.SendWebsocketParentUpdate(_messageHub, docArgs, "Create Parent successful", DataRequestCompleteType.Created);
        }
        */
        private async Task CreateParentActivity(IPipelineArgs<ContentPostParentEntity> args)
        {
            //If parent user was the same as post user, don't create activity
            //if(args.Value.UserId != )
            var entity = new ContentPostActivityEntity()
            {
                Type = ContentPostActivityTypes.Created,
                UserId = args.Value.UserId,
                Message = "Someone replied to your post",
                Url = $"/Threads/{args.Value.ContentPostId}"
            };
            await _pipelineActivityService.Create(entity);
        }
        // SYNC PROCESSORS
    }
}

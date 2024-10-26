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

namespace Accelerate.Features.Content.Pipelines.Chats
{
    public class ContentChatCreatedPipeline : DataCreateEventPipeline<ContentChatEntity>
    {
        IHubContext<BaseHub<ContentChatDocument>, IBaseHubClient<WebsocketMessage<ContentChatDocument>>> _messageHub;
        IElasticService<ContentChatDocument> _elasticService;
        IEntityService<ContentChatEntity> _entityService;
        public ContentChatCreatedPipeline(
            IElasticService<ContentChatDocument> elasticService,
            IEntityService<ContentChatEntity> entityService,
            IHubContext<BaseHub<ContentChatDocument>, IBaseHubClient<WebsocketMessage<ContentChatDocument>>> messageHub)
        {
            _elasticService = elasticService;
            _messageHub = messageHub;
            _entityService = entityService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentChatEntity>>()
            {
                IndexDocument,
                SendWebsocketUpdate
            };
            _processors = new List<PipelineProcessor<ContentChatEntity>>()
            {
            };
        }
        // ASYNC PROCESSORS
        public async Task IndexDocument(IPipelineArgs<ContentChatEntity> args)
        {
            var indexModel = new ContentChatDocument();
            args.Value.Hydrate(indexModel);
            await _elasticService.Index(indexModel);
        }
        public async Task SendWebsocketUpdate(IPipelineArgs<ContentChatEntity> args)
        {
            ContentChatDocument doc;
            var response = await _elasticService.GetDocument<ContentChatDocument>(args.Value.Id.ToString());
            doc = response.Source;

            var payload = new WebsocketMessage<ContentChatDocument>()
            {
                Message = "Create successful",
                Code = 200,
                Data = doc,
                UpdateType = DataRequestCompleteType.Created,
                Group = "Chat",
                Alert = true
            };
            await _messageHub.Clients.All.SendMessage(args.Value.UserId.ToString(), payload);
        }
    }
}

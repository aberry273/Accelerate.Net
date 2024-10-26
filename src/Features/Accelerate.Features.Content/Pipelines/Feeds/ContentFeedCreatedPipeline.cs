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

namespace Accelerate.Features.Content.Pipelines.Feeds
{
    public class ContentFeedCreatedPipeline : DataCreateEventPipeline<ContentFeedEntity>
    {
        IHubContext<BaseHub<ContentFeedDocument>, IBaseHubClient<WebsocketMessage<ContentFeedDocument>>> _messageHub;
        IElasticService<ContentFeedDocument> _elasticService;
        IEntityService<ContentFeedEntity> _entityService;
        public ContentFeedCreatedPipeline(
            IElasticService<ContentFeedDocument> elasticService,
            IEntityService<ContentFeedEntity> entityService,
            IHubContext<BaseHub<ContentFeedDocument>, IBaseHubClient<WebsocketMessage<ContentFeedDocument>>> messageHub)
        {
            _elasticService = elasticService;
            _messageHub = messageHub;
            _entityService = entityService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentFeedEntity>>()
            {
                IndexDocument,
            };
            _processors = new List<PipelineProcessor<ContentFeedEntity>>()
            {
            };
        }
        // ASYNC PROCESSORS
        public async Task IndexDocument(IPipelineArgs<ContentFeedEntity> args)
        {
            var indexModel = new ContentFeedDocument();
            args.Value.Hydrate(indexModel);
            await _elasticService.Index(indexModel);
        }
        public async Task SendWebsocketUpdate(IPipelineArgs<ContentFeedEntity> args)
        {
            ContentFeedDocument doc;
            var response = await _elasticService.GetDocument<ContentFeedDocument>(args.Value.Id.ToString());
            doc = response.Source;

            var payload = new WebsocketMessage<ContentFeedDocument>()
            {
                Message = "Create successful",
                Code = 200,
                Data = doc,
                UpdateType = DataRequestCompleteType.Created,
                Group = "Feed",
                Alert = true
            };
            await _messageHub.Clients.All.SendMessage(args.Value.UserId.ToString(), payload);
        }
    }
}

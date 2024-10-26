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

namespace Accelerate.Features.Content.Pipelines.Lists
{
    public class ContentListCreatedPipeline : DataCreateEventPipeline<ContentListEntity>
    {
        IHubContext<BaseHub<ContentListDocument>, IBaseHubClient<WebsocketMessage<ContentListDocument>>> _messageHub;
        IElasticService<ContentListDocument> _elasticService;
        IEntityService<ContentListEntity> _entityService;
        public ContentListCreatedPipeline(
            IElasticService<ContentListDocument> elasticService,
            IEntityService<ContentListEntity> entityService,
            IHubContext<BaseHub<ContentListDocument>, IBaseHubClient<WebsocketMessage<ContentListDocument>>> messageHub)
        {
            _elasticService = elasticService;
            _messageHub = messageHub;
            _entityService = entityService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentListEntity>>()
            {
                IndexDocument,
            };
            _processors = new List<PipelineProcessor<ContentListEntity>>()
            {
            };
        }
        // ASYNC PROCESSORS
        public async Task IndexDocument(IPipelineArgs<ContentListEntity> args)
        {
            var indexModel = new ContentListDocument();
            args.Value.Hydrate(indexModel);
            await _elasticService.Index(indexModel);
        }
        public async Task SendWebsocketUpdate(IPipelineArgs<ContentListEntity> args)
        {
            ContentListDocument doc;
            var response = await _elasticService.GetDocument<ContentListDocument>(args.Value.Id.ToString());
            doc = response.Source;

            var payload = new WebsocketMessage<ContentListDocument>()
            {
                Message = "Create successful",
                Code = 200,
                Data = doc,
                UpdateType = DataRequestCompleteType.Created,
                Group = "List",
                Alert = true
            };
            await _messageHub.Clients.All.SendMessage(args.Value.UserId.ToString(), payload);
        }
    }
}

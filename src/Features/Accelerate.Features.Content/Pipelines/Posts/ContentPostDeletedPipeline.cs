using Accelerate.Features.Content.Pipelines.Reviews;
using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using Elastic.Clients.Elasticsearch.Ingest;
using Microsoft.AspNetCore.SignalR;

namespace Accelerate.Features.Content.Pipelines.Posts
{
    public class ContentPostDeletedPipeline : DataDeleteEventPipeline<ContentPostEntity>
    {
        IElasticService<ContentPostDocument> _elasticService;
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHub;
        IEntityService<ContentPostEntity> _entityService;
        public ContentPostDeletedPipeline(
            IElasticService<ContentPostDocument> elasticService,
            IEntityService<ContentPostEntity> entityService,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHub)
        {
            _elasticService = elasticService;
            _entityService = entityService;
            _messageHub = messageHub;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostEntity>>()
            {
                DeleteDocument,
            };
        }
        // ASYNC PROCESSORS
        public async Task DeleteDocument(IPipelineArgs<ContentPostEntity> args)
        {
            var response = await _elasticService.DeleteDocument<ContentPostEntity>(args.Value.Id.ToString());
            if(response.IsValidResponse)
            {
                // If a reply
                if (args.Value.ParentId == null) return;
                await ContentPostUtilities.UpdateParentReplies(_elasticService, _entityService, _messageHub, args);
            }
        }
    }
}

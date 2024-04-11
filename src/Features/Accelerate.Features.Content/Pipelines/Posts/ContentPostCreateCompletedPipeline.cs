using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Account.Services;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using Elastic.Clients.Elasticsearch.Ingest;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Accelerate.Features.Content.Pipelines.Posts
{
    public class ContentPostCreateCompletedPipeline : DataCreateCompletedEventPipeline<ContentPostEntity>
    {
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHub;
        IElasticService<ContentPostDocument> _elasticService;
        public ContentPostCreateCompletedPipeline(
            IElasticService<ContentPostDocument> elasticService,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHub)
        {
            _elasticService = elasticService;
            _messageHub = messageHub;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostEntity>>()
            {
                SendWebsocketUpdate
            };
            _processors = new List<PipelineProcessor<ContentPostEntity>>()
            {
            };
        }
        // ASYNC PROCESSORS

        // ASYNC PROCESSORS
        public async Task SendWebsocketUpdate(IPipelineArgs<ContentPostEntity> args)
        {
            ContentPostDocument doc;
            var response = await _elasticService.GetDocument<ContentPostDocument>(args.Value.Id.ToString());
            doc = response.Source;
            // If its a reply to own thread by the user, send the parent as the update instead
            if (doc.PostType == ContentPostType.Thread)
            {
                var parentResponse = await _elasticService.GetDocument<ContentPostDocument>(doc.ParentId.ToString());
                doc = parentResponse.Source;
            }
            var payload = new WebsocketMessage<ContentPostDocument>()
            {
                Message = "Create successful",
                Code = 200,
                Data = doc,
                UpdateType = DataRequestCompleteType.Created,
                Group = "Post",
                Alert = true
            };
            await _messageHub.Clients.All.SendMessage(args.Value.UserId.ToString(), payload);
        }
        // SYNC PROCESSORS
    }
}

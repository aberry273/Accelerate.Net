using Accelerate.Features.Content.Services;
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
using Microsoft.AspNetCore.SignalR;

namespace Accelerate.Features.Content.Pipelines.Posts
{
    public class ContentPostUpdatedPipeline : DataUpdateEventPipeline<ContentPostEntity>
    {
        IElasticService<AccountUserDocument> _accountElasticService;
        IElasticService<ContentPostDocument> _elasticService;
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHub;
        public ContentPostUpdatedPipeline(
            IElasticService<ContentPostDocument> elasticService,
            IElasticService<AccountUserDocument> accountElasticService,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHub)
        {
            _elasticService = elasticService;
            _accountElasticService = accountElasticService;
            _messageHub = messageHub;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostEntity>>()
            {
                UpdateDocument,
            };
        }
        // ASYNC PROCESSORS
        public async Task UpdateDocument(IPipelineArgs<ContentPostEntity> args)
        {
            var user = await _accountElasticService.GetDocument<AccountUserDocument>(args.Value.UserId.GetValueOrDefault().ToString());
            var indexModel = new ContentPostDocument();
            args.Value.HydrateDocument(indexModel, user?.Source?.Username);
            await _elasticService.UpdateOrCreateDocument(indexModel, args.Value.Id.ToString());

            //
            await SendWebsocketUpdate(args);
        }
        public async Task SendWebsocketUpdate(IPipelineArgs<ContentPostEntity> args)
        {
            var doc = await _elasticService.GetDocument<ContentPostDocument>(args.Value.Id.ToString());
            var payload = new WebsocketMessage<ContentPostDocument>()
            {
                Message = "Update successful",
                Code = 200,
                Data = doc.Source,
                UpdateType = DataRequestCompleteType.Updated,
                Group = "Post"
            };
            var userConnections = HubClientConnectionsSingleton.GetUserConnections(args.Value.UserId.ToString());
            await _messageHub.Clients.Clients(userConnections).SendMessage(args.Value.UserId.ToString(), payload);
        }
    }
}

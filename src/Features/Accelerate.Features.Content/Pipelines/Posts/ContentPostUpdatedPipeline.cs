using Accelerate.Features.Content.Pipelines.Reviews;
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
            // If a reply
            if (args.Value.ParentId == null) return;
            await UpdateParentDocument(indexModel, args);
        }

        private async Task UpdateParentDocument(ContentPostDocument childDoc, IPipelineArgs<ContentPostEntity> args)
        {
            try
            {
                var parentResponse = await _elasticService.GetDocument<ContentPostDocument>(args.Value.ParentId.ToString());
                var parentDoc = parentResponse.Source;
                if (!parentResponse.IsValidResponse || parentDoc == null) return;

                // Update threads
                if (parentDoc.UserId == args.Value.UserId)
                {
                    if (parentDoc.Threads == null) parentDoc.Threads = new List<ContentPostDocument>();
                    var index = parentDoc.Threads.IndexOf(childDoc);
                    if (index > -1)
                        parentDoc.Threads[index] = childDoc;
                    else
                        parentDoc.Threads.Add(childDoc);
                }
                await _elasticService.UpdateDocument(parentDoc, parentDoc.Id.ToString());
            }
            catch(Exception ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
            }
        }
    }
}

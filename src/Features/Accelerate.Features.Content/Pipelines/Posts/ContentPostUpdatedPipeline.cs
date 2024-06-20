using Accelerate.Features.Content.Pipelines.Actions;
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
using Accelerate.Foundations.Content.Hydrators;
using Microsoft.AspNetCore.SignalR;
using Accelerate.Foundations.Content.Services;

namespace Accelerate.Features.Content.Pipelines.Posts
{
    public class ContentPostUpdatedPipeline : DataUpdateEventPipeline<ContentPostEntity>
    {
        IContentPostService _contentPostService;
        IElasticService<AccountUserDocument> _accountElasticService;
        IElasticService<ContentPostDocument> _elasticService;
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHub;
        public ContentPostUpdatedPipeline(
            IContentPostService contentPostService,
            IElasticService<ContentPostDocument> elasticService,
            IElasticService<AccountUserDocument> accountElasticService,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHub)
        {
            _contentPostService = contentPostService;
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
            var user = await _accountElasticService.GetDocument<AccountUserDocument>(args.Value.UserId.ToString());
            var indexModel = new ContentPostDocument();
            
            var profile = new ContentPostUserSubdocument()
            {
                Username = user?.Source?.Username,
                Image = user?.Source?.Image
            };

            args.Value.Hydrate(indexModel, profile);
            await _elasticService.UpdateOrCreateDocument(indexModel, args.Value.Id.ToString());
           
            await UpdateParentDocument(indexModel, args);
        }

        private async Task UpdateParentDocument(ContentPostDocument childDoc, IPipelineArgs<ContentPostEntity> args)
        {
            try
            {
                var parentPost = _contentPostService.GetPostParent(args.Value);
                // If a reply
                if (parentPost == null || parentPost.ParentId == null) return;
                var parentResponse = await _elasticService.GetDocument<ContentPostDocument>(parentPost.ParentId.ToString());
                var parentDoc = parentResponse.Source;
                if (!parentResponse.IsValidResponse || parentDoc == null) return;

                // Update threads
                if (parentDoc.UserId == args.Value.UserId)
                {
                    if (parentDoc.Pages == null) parentDoc.Pages = new List<ContentPostDocument>();
                    var index = parentDoc.Pages.IndexOf(childDoc);
                    if (index > -1)
                        parentDoc.Pages[index] = childDoc;
                    else
                        parentDoc.Pages.Add(childDoc);
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

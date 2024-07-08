using Accelerate.Features.Content.Pipelines.Actions;
using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.EventBus;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Content.Services;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.EventPipelines.Services;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using Elastic.Clients.Elasticsearch.Ingest;
using Microsoft.AspNetCore.SignalR;

namespace Accelerate.Features.Content.Pipelines.Posts
{
    public class ContentPostDeletedPipeline : DataDeleteEventPipeline<ContentPostEntity>
    {
        IContentPostService _contentPostService;
        IElasticService<ContentPostDocument> _elasticService;
        IEntityPipelineService<ContentPostActivityEntity, IContentPostActivityBus> _pipelineActivityService;
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHub;
        IEntityService<ContentPostEntity> _entityService;
        public ContentPostDeletedPipeline(
            IContentPostService contentPostService,
            IElasticService<ContentPostDocument> elasticService,
            IEntityService<ContentPostEntity> entityService,
            IEntityPipelineService<ContentPostActivityEntity, IContentPostActivityBus> pipelineActivityService,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHub)
        {
            _pipelineActivityService = pipelineActivityService;
            _contentPostService = contentPostService;
            _elasticService = elasticService;
            _entityService = entityService;
            _messageHub = messageHub;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostEntity>>()
            {
                DeleteDocument,
                CreatePostActivity
            };
        }
        private async Task CreatePostActivity(IPipelineArgs<ContentPostEntity> args)
        {
            var entity = new ContentPostActivityEntity()
            {
                Type = ContentPostActivityTypes.Deleted,
                UserId = args.Value.UserId,
                Message = "Post deleted"
            };
            await _pipelineActivityService.Create(entity);
        }
        // ASYNC PROCESSORS
        public async Task DeleteDocument(IPipelineArgs<ContentPostEntity> args)
        {
            var response = await _elasticService.DeleteDocument<ContentPostEntity>(args.Value.Id.ToString());
            if (!response.IsValidResponse) return;

            await SendWebsocketUpdate(args);
        }
        public async Task SendWebsocketUpdate(IPipelineArgs<ContentPostEntity> args)
        {
            var userId = args.Value.UserId.ToString();
            var id = args.Value.Id;

            // Get doc, sned parent ID if its a self rpely
            var payload = new WebsocketMessage<ContentPostDocument>()
            {
                Message = "Delete successful",
                Code = 200,
                Data = new ContentPostDocument()
                {
                    Id = id,
                },
                UpdateType = DataRequestCompleteType.Deleted,
                Group = "Post",
                Alert = true
            };
            var userConnections = HubClientConnectionsSingleton.GetUserConnections(userId);
            await _messageHub.Clients.Clients(userConnections).SendMessage(userId, payload);
        }

        private async Task UpdateParentDocument(IPipelineArgs<ContentPostEntity> args)
        {
            var parentPost = _contentPostService.GetPostParent(args.Value);
            // If a reply
            if (parentPost == null || parentPost.ParentId == null) return;
            var parentResponse = await _elasticService.GetDocument<ContentPostDocument>(parentPost.ParentId.ToString());
            var parentDoc = parentResponse.Source;
            if (!parentResponse.IsValidResponse || parentDoc == null) return;
            // Update reply count
            //parentDoc.Replies = _contentPostService.GetReplyCount(parentPost.ParentId.GetValueOrDefault());
            // Update threads
            if (parentDoc.UserId == args.Value.UserId)
            {
                if (parentDoc.Pages == null) return;
                var index = parentDoc.Pages.FindIndex(x => x.Id == args.Value.Id);
                parentDoc.Pages.RemoveAt(index);
            }
            await _elasticService.UpdateDocument(parentDoc, parentDoc.Id.ToString());
        }
    }
}

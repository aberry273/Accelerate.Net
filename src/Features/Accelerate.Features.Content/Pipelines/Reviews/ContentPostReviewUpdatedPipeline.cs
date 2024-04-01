using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Account.Services;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.MSearch;
using Elastic.Clients.Elasticsearch.Ingest;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.AspNetCore.SignalR;

namespace Accelerate.Features.Content.Pipelines.Reviews
{
    public class ContentPostReviewUpdatedPipeline : DataUpdateEventPipeline<ContentPostReviewEntity>
    {
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHubPosts;
        IHubContext<BaseHub<ContentPostReviewDocument>, IBaseHubClient<WebsocketMessage<ContentPostReviewDocument>>> _messageHub;
        IElasticService<ContentPostDocument> _elasticPostService;
        IElasticService<ContentPostReviewDocument> _elasticService;
        public ContentPostReviewUpdatedPipeline(
            IElasticService<ContentPostReviewDocument> elasticService,
            IElasticService<ContentPostDocument> elasticPostService,
            IHubContext<BaseHub<ContentPostReviewDocument>, IBaseHubClient<WebsocketMessage<ContentPostReviewDocument>>> messageHub,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHubPosts)
        {
            _messageHub = messageHub;
            _messageHubPosts = messageHubPosts;
            _elasticService = elasticService;
            _elasticPostService = elasticPostService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostReviewEntity>>()
            {
                IndexDocument,
                UpdatePostIndex
            };
            _processors = new List<PipelineProcessor<ContentPostReviewEntity>>()
            {
            };
        }
        // ASYNC PROCESSORS
        public async Task IndexDocument(IPipelineArgs<ContentPostReviewEntity> args)
        {
            var indexModel = new ContentPostReviewDocument();
            args.Value.HydrateDocument(indexModel);
            var result = await _elasticService.UpdateOrCreateDocument(indexModel, args.Value.Id.ToString());
            // Run directly in same function as reviews are simple types and will not be extended via pipelines
            if(result.IsValidResponse)
            {
                var docArgs = new PipelineArgs<ContentPostReviewDocument>()
                {
                    Value = indexModel
                };
                await this.SendWebsocketUpdate(docArgs);
            }
        }
        public async Task SendWebsocketUpdate(IPipelineArgs<ContentPostReviewDocument> args)
        {
            var payload = new WebsocketMessage<ContentPostReviewDocument>()
            {
                Message = "Update review successful",
                Code = 200,
                Data = args.Value,
                UpdateType = DataRequestCompleteType.Updated,
                Group = "Review",
            };
            await _messageHub.Clients.All.SendMessage(args.Value.UserId.ToString(), payload);
        }
        /// <summary>
        /// TODO: REFACTOR THIS ONCE POC READY
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task UpdatePostIndex(IPipelineArgs<ContentPostReviewEntity> args)
        {
            //get doc
            var fetchResponse = await _elasticPostService.GetDocument<ContentPostDocument>(args.Value.ContentPostId.ToString());
            var contentPostDocument = fetchResponse.Source;
            // fetch reviews
            var reviewResults = await GetPostReviewsQuery(args);
            var agrees = reviewResults.Documents?.Count(x => x.Agree == true);
            var disagrees = reviewResults.Documents?.Count(x => x.Disagree == true);
            var likes = reviewResults.Documents?.Count(x => x.Like == true);
             
            contentPostDocument.Agrees = agrees ?? 0;
            contentPostDocument.Disagrees = disagrees ?? 0;
            contentPostDocument.Likes = likes ?? 0;
            // Update document index
            await _elasticPostService.UpdateDocument(contentPostDocument, args.Value?.ContentPostId.ToString());

            // Send websocket request
            await SendWebsocketPostUpdate(args.Value?.UserId.ToString(), contentPostDocument);
        }
        private async Task<SearchResponse<ContentPostReviewEntity>> GetPostReviewsQuery(IPipelineArgs<ContentPostReviewEntity> args)
        {
            var query = new QueryDescriptor<ContentPostReviewEntity>();
            query.MatchAll();

            query.Term(x =>
                x.ContentPostId.Suffix("keyword"),
                args.Value.ContentPostId
            );

            return await _elasticService.Search(query);
        }

        public async Task SendWebsocketPostUpdate(string userId, ContentPostDocument doc)
        { 
            var payload = new WebsocketMessage<ContentPostDocument>()
            {
                Message = "Update successful",
                Code = 200,
                Data = doc,
                UpdateType = DataRequestCompleteType.Updated,
                Group = "Post"
            };
            var userConnections = HubClientConnectionsSingleton.GetUserConnections(userId);
            await _messageHubPosts.Clients.Clients(userConnections).SendMessage(userId, payload);
        }


    }
}

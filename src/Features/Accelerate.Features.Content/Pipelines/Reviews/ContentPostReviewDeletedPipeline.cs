using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Ingest;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.AspNetCore.SignalR;

namespace Accelerate.Features.Content.Pipelines.Posts
{
    public class ContentPostReviewDeletedPipeline : DataDeleteEventPipeline<ContentPostReviewEntity>
    {
        IHubContext<BaseHub<ContentPostReviewDocument>, IBaseHubClient<WebsocketMessage<ContentPostReviewDocument>>> _messageHub;
        IElasticService<ContentPostReviewDocument> _elasticService;
        IElasticService<ContentPostDocument> _elasticPostService;
        public ContentPostReviewDeletedPipeline(
            IElasticService<ContentPostDocument> elasticPostService,
            IElasticService<ContentPostReviewDocument> elasticService,
            IHubContext<BaseHub<ContentPostReviewDocument>, IBaseHubClient<WebsocketMessage<ContentPostReviewDocument>>> messageHub)
        {
            _elasticService = elasticService;
            _elasticPostService = elasticPostService;
            _messageHub = messageHub;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostReviewEntity>>()
            {
                DeleteDocument,
                UpdatePostIndex
            };
        }
        // ASYNC PROCESSORS
        public async Task DeleteDocument(IPipelineArgs<ContentPostReviewEntity> args)
        {
            var indexResponse = await _elasticService.DeleteDocument<ContentPostReviewEntity>(args.Value.Id.ToString());

            var docArgs = new PipelineArgs<ContentPostReviewDocument>()
            {
                Value = new ContentPostReviewDocument()
                {
                    Id = args.Value.Id,
                }
            };
            await SendWebsocketUpdate(docArgs);
        }
        public async Task UpdatePostIndex(IPipelineArgs<ContentPostReviewEntity> args)
        {
            var reviewResults = await GetPostReviewsQuery(args);
            var agrees = reviewResults.Documents?.Count(x => x.Agree == true);
            var disagrees = reviewResults.Documents?.Count(x => x.Disagree == true);
            var likes = reviewResults.Documents?.Count(x => x.Like == true);

            var indexModel = new ContentPostDocument();
            indexModel.Agrees = agrees ?? 0;
            indexModel.Disagrees = disagrees ?? 0;
            indexModel.Likes = likes ?? 0;
            await _elasticPostService.UpdateDocument(indexModel, args.Value?.ContentPostId.ToString());
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
        // TODO: Potentially remove or add, unsure of userbased state mgmt yet
        public async Task SendWebsocketUpdate(IPipelineArgs<ContentPostReviewDocument> args)
        {
            var payload = new WebsocketMessage<ContentPostReviewDocument>()
            {
                Message = "Delete review successful",
                Code = 200,
                Data = args.Value,
                UpdateType = DataRequestCompleteType.Deleted,
                Group = "Review",
            };
            await _messageHub.Clients.All.SendMessage(args.Value.UserId.ToString(), payload);
        }
    }
}

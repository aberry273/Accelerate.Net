using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Account.Services;
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
using System;

namespace Accelerate.Features.Content.Pipelines.Reviews
{
    public class ContentPostReviewCreatedPipeline : DataCreateEventPipeline<ContentPostReviewEntity>
    {
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHubPosts;
        IHubContext<BaseHub<ContentPostReviewDocument>, IBaseHubClient<WebsocketMessage<ContentPostReviewDocument>>> _messageHub;
        IEntityService<ContentPostReviewEntity> _entityService;
        IElasticService<ContentPostReviewDocument> _elasticService;
        IElasticService<ContentPostDocument> _elasticPostService;
        public ContentPostReviewCreatedPipeline(
            IElasticService<ContentPostReviewDocument> elasticService,
            IElasticService<ContentPostDocument> elasticPostService,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHubPosts,
            IEntityService<ContentPostReviewEntity> entityService,
            IHubContext<BaseHub<ContentPostReviewDocument>, IBaseHubClient<WebsocketMessage<ContentPostReviewDocument>>> messageHub)
        {
            _elasticService = elasticService;
            _elasticPostService = elasticPostService;
            _messageHub = messageHub;
            _entityService = entityService;
            _messageHubPosts = messageHubPosts;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostReviewEntity>>()
            {
                IndexDocument,
                UpdatePostIndex,
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
            await _elasticService.Index(indexModel);

            var docArgs = new PipelineArgs<ContentPostReviewDocument>()
            {
                Value = indexModel
            };
            await ContentPostReviewUtilities.SendWebsocketReviewUpdate(_messageHub, docArgs, "Create review successful", DataRequestCompleteType.Created);
        }
        public async Task UpdatePostIndex(IPipelineArgs<ContentPostReviewEntity> args)
        {
            // fetch reviews
            var reviewsDoc = ContentPostReviewUtilities.GetReviews(_entityService, args);
            var fetchResponse = await _elasticPostService.GetDocument<ContentPostDocument>(args.Value.ContentPostId.ToString());
            var contentPostDocument = fetchResponse.Source;
            contentPostDocument.Reviews = reviewsDoc;
            contentPostDocument.UpdatedOn = DateTime.Now;
            await _elasticPostService.UpdateDocument(contentPostDocument, args.Value?.ContentPostId.ToString());

            // Send websocket request
            await ContentPostUtilities.SendWebsocketPostUpdate(_messageHubPosts, args.Value?.UserId.ToString(), contentPostDocument, DataRequestCompleteType.Updated);
        } 
          

        // SYNC PROCESSORS
    }
}

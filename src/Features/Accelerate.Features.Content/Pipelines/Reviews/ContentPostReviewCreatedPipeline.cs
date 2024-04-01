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
        IHubContext<BaseHub<ContentPostReviewDocument>, IBaseHubClient<WebsocketMessage<ContentPostReviewDocument>>> _messageHub;
        IElasticService<ContentPostReviewDocument> _elasticService;
        IElasticService<ContentPostDocument> _elasticPostService;
        public ContentPostReviewCreatedPipeline(
            IElasticService<ContentPostReviewDocument> elasticService,
            IElasticService<ContentPostDocument> elasticPostService,
            IHubContext<BaseHub<ContentPostReviewDocument>, IBaseHubClient<WebsocketMessage<ContentPostReviewDocument>>> messageHub)
        {
            _elasticService = elasticService;
            _elasticPostService = elasticPostService;
            _messageHub = messageHub;
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
            await _elasticService.Index(indexModel);

            var docArgs = new PipelineArgs<ContentPostReviewDocument>()
            {
                Value = indexModel
            };
            await SendWebsocketUpdate(docArgs);
        }
        // TODO: Rewrite as aggregation/multisearch count
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
                Message = "Create review successful",
                Code = 200,
                Data = args.Value,
                UpdateType = DataRequestCompleteType.Deleted,
                Group = "Review",
            };
            await _messageHub.Clients.All.SendMessage(args.Value.UserId.ToString(), payload);
        }

        // SYNC PROCESSORS
    }
}

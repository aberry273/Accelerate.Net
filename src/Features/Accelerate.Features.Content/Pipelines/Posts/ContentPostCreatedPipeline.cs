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
using Elastic.Clients.Elasticsearch.Ingest;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Accelerate.Features.Content.Pipelines.Posts
{
    public class ContentPostCreatedPipeline : DataCreateEventPipeline<ContentPostEntity>
    {
        IElasticService<AccountUserDocument> _accountElasticService;
        IElasticService<ContentPostDocument> _elasticService;
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHub;
        public ContentPostCreatedPipeline(
            IElasticService<ContentPostDocument> elasticService,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHub,
            IElasticService<AccountUserDocument> accountElasticService)
        {
            _elasticService = elasticService;
            _messageHub = messageHub;
            _accountElasticService = accountElasticService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostEntity>>()
            {
                IndexDocument,
                UpdateTargetPostRepliesCount,
                //SendWebsocketUpdate
            };
            _processors = new List<PipelineProcessor<ContentPostEntity>>()
            {
            };
        }
        // ASYNC PROCESSORS
        public async Task IndexDocument(IPipelineArgs<ContentPostEntity> args)
        {
            var user = await _accountElasticService.GetDocument<AccountUserDocument>(args.Value.UserId.GetValueOrDefault().ToString());
            var indexModel = new ContentPostDocument();
            args.Value.HydrateDocument(indexModel, user?.Source?.Username);
            await _elasticService.Index(indexModel);
            
            // TODO: Review if completed pipelines are really necessary
            // If there are enough separate requests to different systems to run after core processes
            // If its only ever one 'final request', it can be directly called from another method
            await SendWebsocketUpdate(args);
        }

        // ASYNC PROCESSORS
        public async Task SendWebsocketUpdate(IPipelineArgs<ContentPostEntity> args)
        {
            var doc = await _elasticService.GetDocument<ContentPostDocument>(args.Value.Id.ToString());
            var payload = new WebsocketMessage<ContentPostDocument>()
            {
                Message = "Create successful",
                Code = 200,
                Data = doc.Source,
                UpdateType = DataRequestCompleteType.Created,
                Group = "Post"
            };
            await _messageHub.Clients.All.SendMessage(args.Value.UserId.ToString(), payload);
        }

        // TODO: Rewrite as aggregation/multisearch count
        public async Task UpdateTargetPostRepliesCount(IPipelineArgs<ContentPostEntity> args)
        {
            // If this is not a reply to another thread, ignore
            if (args.Value.ParentId == null) return;

            var reviewResults = await GetPostReviewsQuery(args);
            var replies = reviewResults.Documents.Count;

            var indexModel = new ContentPostDocument();
            indexModel.Replies = (int)replies;
            await _elasticService.UpdateDocument(indexModel, args.Value.ParentId.ToString());
        }
        private async Task<CountResponse> GetPostReviewsCountQuery(IPipelineArgs<ContentPostEntity> args)
        {
            var query = new CountRequestDescriptor<ContentPostEntity>();
            query.Query(q => q.Term(x => x.ParentId.Suffix("keyword"), args.Value.ParentId));
             
            return await _elasticService.Count<ContentPostEntity>(a => a.Query(q => q.Term(x => x.ParentId.Suffix("keyword"), args.Value.ParentId)));
        }
        // SYNC PROCESSORS
        private async Task<SearchResponse<ContentPostEntity>> GetPostReviewsQuery(IPipelineArgs<ContentPostEntity> args)
        {
            var query = new QueryDescriptor<ContentPostEntity>();
            query.MatchAll();

            query.Term(x => x.ParentId.Suffix("keyword"), args.Value.ParentId);

            return await _elasticService.Search(query);
        }
        // SYNC PROCESSORS
    }
}

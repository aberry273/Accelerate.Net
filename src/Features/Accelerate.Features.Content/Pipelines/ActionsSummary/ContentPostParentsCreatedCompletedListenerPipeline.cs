using Accelerate.Foundations.Users.Models;
using Accelerate.Foundations.Users.Services;
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
using Accelerate.Foundations.Content.Hydrators;
using System;
using Accelerate.Foundations.Common.Models;

namespace Accelerate.Features.Content.Pipelines.ActionsSummary
{
    public class ContentPostParentsCreatedCompletedListenerPipeline : DataCreateCompletedEventPipeline<ContentPostParentEntity>
    {
        IHubContext<BaseHub<ContentPostActionsSummaryDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsSummaryDocument>>> _messageHub;
        IEntityService<ContentPostParentEntity> _entityParentService;
        IElasticService<ContentPostDocument> _elasticPostService;
        IElasticService<ContentPostActionsSummaryDocument> _elasticService;
        IElasticService<ContentPostActionsDocument> _elasticActionsService;
        IEntityService<ContentPostActionsSummaryEntity> _entityService;
        public ContentPostParentsCreatedCompletedListenerPipeline(
            IEntityService<ContentPostActionsSummaryEntity> entityService,
            IEntityService<ContentPostParentEntity> entityParentService,
            IElasticService<ContentPostDocument> elasticPostService,
            IElasticService<ContentPostActionsSummaryDocument> elasticService,
            IElasticService<ContentPostActionsDocument> elasticActionsService,
            IHubContext<BaseHub<ContentPostActionsSummaryDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsSummaryDocument>>> messageHub)
        {
            _entityService = entityService;
            _entityParentService = entityParentService;
            _elasticPostService = elasticPostService;
            _elasticService = elasticService;
            _elasticActionsService = elasticActionsService;
            _messageHub = messageHub;

            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostParentEntity>>()
            {
                //UpdateIndex,

                //UpdatePostReviewIndex
            };
        }
        private async Task<ContentPostActionsSummaryEntity> CreateActionSummaryEntity(IPipelineArgs<ContentPostParentEntity> args)
        {
            var entity = new ContentPostActionsSummaryEntity()
            {
                ContentPostId = args.Value.ContentPostId,
            };

            var guid = await _entityService.CreateWithGuid(entity);

            return _entityService.Get(guid.GetValueOrDefault());
        }
        public async Task UpdateIndex(IPipelineArgs<ContentPostParentEntity> args)
        {
            var entity = _entityService
                .Find(x => x.ContentPostId == args.Value.ParentId, 0, 1)
                .FirstOrDefault();

            if (entity == null)
            {
                entity = await this.CreateActionSummaryEntity(args);
            }

            var response = await _elasticService.GetDocument<ContentPostActionsSummaryDocument>(entity.Id.ToString());
            var summary = response.Source;
            var newDoc = summary == null;
            if (newDoc)
            {
                summary = new ContentPostActionsSummaryDocument()
                {
                    Id = entity.Id,
                    ContentPostId = args.Value.ParentId.GetValueOrDefault(),
                    UserId = args.Value.UserId,
                };
            }

            summary.Replies = _entityParentService.Count(x => x.ParentId == args.Value.ParentId);

            if (newDoc)
            {
                await _elasticService.Index(summary);
                await ContentPostActionsSummaryUtilities.SendWebsocketActionsSummaryUpdate(_messageHub, summary, "Created", DataRequestCompleteType.Created);
            }
            else
            {
                await _elasticService.UpdateDocument(summary, summary.Id.ToString());
                await ContentPostActionsSummaryUtilities.SendWebsocketActionsSummaryUpdate(_messageHub, summary, "Updated", DataRequestCompleteType.Updated);
            }
            //do stuff
            var post = new ContentPostDocument()
            {
                Id = args.Value.ContentPostId,
                //ActionsTotals = summary
            };
            await _elasticPostService.UpdateDocument<ContentPostDocument>(post, args.Value.ContentPostId.ToString());
        }

        /// <summary>
        /// Update the content post index to include the positive review if available
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>P
        public async Task UpdateostReviewIndex(IPipelineArgs<ContentPostParentEntity> args)
        {
            var action = await GetUserAction(args);

            //Get the action associated with the reply
            if (action == null)
            {
                return;
            }

            //Update the post to include the vote if available
            var post = new ContentPostDocument()
            {
                Id = args.Value.ContentPostId
            };
            if(action.Agree.GetValueOrDefault()) {
                //post.ParentVote = "Agree";
            }
            if(action.Disagree.GetValueOrDefault()) {
                //post.ParentVote = "Disagree";
            }
            var response = await _elasticPostService.UpdateDocument<ContentPostDocument>(post, args.Value.ContentPostId.ToString());
        }
        private async Task<ContentPostDocument> GetDocument(Guid id)
        {
            var result = await _elasticPostService.GetDocument<ContentPostDocument>(id.ToString());
            if (!result.IsSuccess() || result.Source == null) return null;
            return result.Source;
        }
        public async Task<ContentPostActionsDocument> GetUserAction(IPipelineArgs<ContentPostParentEntity> args)
        {
            var query = new QueryDescriptor<ContentPostActionsDocument>();
            if (args.Value.ContentPostId != null && args.Value.UserId != null)
            {
                query.MatchAll();
                query
                    .Term(x => x.UserId.Suffix("keyword"), args.Value.UserId)
                    .Term(x => x.ContentPostId.Suffix("keyword"), args.Value.ParentId)
                ;
            }
            var results = await _elasticActionsService.Search<ContentPostActionsDocument>(query, 0, 1);

            //Get the action associated with the reply
            if (!results.IsSuccess() || results.Documents == null)
            {
                return null;
            }
            return results.Documents.FirstOrDefault();
        }
    }
}

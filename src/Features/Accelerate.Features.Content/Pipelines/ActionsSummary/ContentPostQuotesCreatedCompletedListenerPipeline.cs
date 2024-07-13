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
using Accelerate.Foundations.Content.Hydrators;
using System;

namespace Accelerate.Features.Content.Pipelines.ActionsSummary
{
    public class ContentPostQuotesCreatedCompletedListenerPipeline : DataCreateCompletedEventPipeline<ContentPostQuoteEntity>
    {
        IHubContext<BaseHub<ContentPostActionsSummaryDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsSummaryDocument>>> _messageHub;
        IEntityService<ContentPostQuoteEntity> _entityQuoteService;
        IElasticService<ContentPostActionsSummaryDocument> _elasticService;
        IElasticService<ContentPostDocument> _elasticPostService;
        IEntityService<ContentPostActionsSummaryEntity> _entityService;
        public ContentPostQuotesCreatedCompletedListenerPipeline(
            IEntityService<ContentPostActionsSummaryEntity> entityService, 
            IEntityService<ContentPostQuoteEntity> entityQuoteService,
            IElasticService<ContentPostDocument> elasticPostService,
            IElasticService<ContentPostActionsSummaryDocument> elasticService,
            IHubContext<BaseHub<ContentPostActionsSummaryDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsSummaryDocument>>> messageHub)
        {
            _entityService = entityService;
            _entityQuoteService = entityQuoteService;
            _elasticPostService = elasticPostService;
            _elasticService = elasticService;
            _messageHub = messageHub;

            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostQuoteEntity>>()
            {
                UpdateIndex
            };
        }

        private async Task<ContentPostActionsSummaryEntity> CreateActionSummaryEntity(IPipelineArgs<ContentPostQuoteEntity> args)
        {
            var entity = new ContentPostActionsSummaryEntity()
            {
                ContentPostId = args.Value.ContentPostId,
            };

            var guid = await _entityService.CreateWithGuid(entity);

            return _entityService.Get(guid.GetValueOrDefault());
        }

        public async Task UpdateIndex(IPipelineArgs<ContentPostQuoteEntity> args)
        {
            var entity = _entityService
                .Find(x => x.ContentPostId == args.Value.QuotedContentPostId, 0, 1)
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
                    ContentPostId = args.Value.QuotedContentPostId,
                    UserId = args.Value.UserId,
                };
            }

            summary.Quotes = _entityQuoteService.Count(x => x.QuotedContentPostId == args.Value.QuotedContentPostId);

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
                ActionsTotals = summary
            };
            await _elasticPostService.UpdateDocument<ContentPostDocument>(post, args.Value.ContentPostId.ToString());
        }
    }
}

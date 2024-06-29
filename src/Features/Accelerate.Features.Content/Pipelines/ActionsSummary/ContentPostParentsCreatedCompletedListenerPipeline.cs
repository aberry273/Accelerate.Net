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
    public class ContentPostParentsCreatedCompletedListenerPipeline : DataCreateCompletedEventPipeline<ContentPostParentEntity>
    {
        IHubContext<BaseHub<ContentPostActionsSummaryDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsSummaryDocument>>> _messageHub;
        IEntityService<ContentPostParentEntity> _entityParentService;
        IElasticService<ContentPostActionsSummaryDocument> _elasticService;
        IEntityService<ContentPostActionsSummaryEntity> _entityService;
        public ContentPostParentsCreatedCompletedListenerPipeline(
            IEntityService<ContentPostActionsSummaryEntity> entityService, 
            IEntityService<ContentPostParentEntity> entityParentService,
            IElasticService<ContentPostActionsSummaryDocument> elasticService,
            IHubContext<BaseHub<ContentPostActionsSummaryDocument>, IBaseHubClient<WebsocketMessage<ContentPostActionsSummaryDocument>>> messageHub)
        {
            _entityService = entityService;
            _entityParentService = entityParentService;
            _elasticService = elasticService;
            _messageHub = messageHub;

            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostParentEntity>>()
            {
                UpdateIndex
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
            var doc = response.Source;
            var newDoc = doc == null;
            if (newDoc)
            {
                doc = new ContentPostActionsSummaryDocument()
                {
                    Id = entity.Id,
                    ContentPostId = args.Value.ParentId.GetValueOrDefault(),
                    UserId = args.Value.UserId,
                };
            }

            doc.Replies = _entityParentService.Count(x => x.ParentId == args.Value.ParentId);

            if (newDoc)
            {
                await _elasticService.Index(doc);
                await ContentPostActionsSummaryUtilities.SendWebsocketActionsSummaryUpdate(_messageHub, doc, "Created", DataRequestCompleteType.Created);
            }
            else
            {
                await _elasticService.UpdateDocument(doc, doc.Id.ToString());
                await ContentPostActionsSummaryUtilities.SendWebsocketActionsSummaryUpdate(_messageHub, doc, "Updated", DataRequestCompleteType.Updated);
            }
            //do stuff

        }
    }
}

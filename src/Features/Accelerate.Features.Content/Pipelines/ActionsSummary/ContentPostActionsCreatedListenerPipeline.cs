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
    public class ContentPostActionsCreatedListenerPipeline : DataCreateEventPipeline<ContentPostActionsEntity>
    {
        IEntityService<ContentPostActionsEntity> _entityActionsService;
        IEntityService<ContentPostActionsSummaryEntity> _entityService;
        IElasticService<ContentPostActionsDocument> _elasticService;
        public ContentPostActionsCreatedListenerPipeline(IEntityService<ContentPostActionsSummaryEntity> entityService, IEntityService<ContentPostActionsEntity> entityActionsService)
        {
            _entityService = entityService;
             _entityActionsService = entityActionsService;

            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostActionsEntity>>()
            {
                UpdateEntity,
                UpdateIndex,
            };
        }
        public async Task UpdateEntity(IPipelineArgs<ContentPostActionsEntity> args)
        {
            var summary = ContentPostActionsSummaryUtilities.GetActionSummaryEntity(_entityService, args);
            if(summary == null)
            {
                summary = ContentPostActionsSummaryUtilities.CreateActionSummaryEntity(args);
                var result = await ContentPostActionsSummaryUtilities.CreateActionSummary(_entityService, summary);
            }
            else
            {
                var entityCounts = ContentPostActionsSummaryUtilities.GetActionCounts(_entityActionsService, args);
                summary.Agrees = entityCounts.Agrees.GetValueOrDefault();
                summary.Disagrees = entityCounts.Disagrees.GetValueOrDefault();
            }
            args.Params = summary;
            //do stuff
        }
        public async Task UpdateIndex(IPipelineArgs<ContentPostActionsEntity> args)
        {
            
            var indexModel = new ContentPostActionsSummaryDocument();
            //do stuff
        }
    }
}

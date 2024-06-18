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
    public class ContentPostActionsUpdatedListenerPipeline : DataUpdateEventPipeline<ContentPostActionsEntity>
    {
        public ContentPostActionsUpdatedListenerPipeline()
        {
            // run summary update of action event

            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostActionsEntity>>()
            {
                PerformUpdate,
            };
        }
        public async Task PerformUpdate(IPipelineArgs<ContentPostActionsEntity> args)
        {
            var indexModel = new ContentPostActionsSummaryDocument();
            //do stuff
        }
    }
}

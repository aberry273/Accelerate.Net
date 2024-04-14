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

namespace Accelerate.Features.Content.Pipelines.Activities
{
    public class ContentPostActivityCreatedPipeline : DataCreateEventPipeline<ContentPostActivityEntity>
    {
        IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> _messageHubPosts;
        IHubContext<BaseHub<ContentPostActivityDocument>, IBaseHubClient<WebsocketMessage<ContentPostActivityDocument>>> _messageHub;
        IEntityService<ContentPostActivityEntity> _entityService;
        IElasticService<ContentPostActivityDocument> _elasticService;
        IElasticService<ContentPostDocument> _elasticPostService;
        public ContentPostActivityCreatedPipeline(
            IElasticService<ContentPostActivityDocument> elasticService,
            IElasticService<ContentPostDocument> elasticPostService,
            IHubContext<BaseHub<ContentPostDocument>, IBaseHubClient<WebsocketMessage<ContentPostDocument>>> messageHubPosts,
            IEntityService<ContentPostActivityEntity> entityService,
            IHubContext<BaseHub<ContentPostActivityDocument>, IBaseHubClient<WebsocketMessage<ContentPostActivityDocument>>> messageHub)
        {
            _elasticService = elasticService;
            _elasticPostService = elasticPostService;
            _messageHub = messageHub;
            _entityService = entityService;
            _messageHubPosts = messageHubPosts;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostActivityEntity>>()
            {
                IndexDocument,
                UpdatePostIndex,
            };
            _processors = new List<PipelineProcessor<ContentPostActivityEntity>>()
            {
            };
        }
        // ASYNC PROCESSORS
        public async Task IndexDocument(IPipelineArgs<ContentPostActivityEntity> args)
        {
            var indexModel = new ContentPostActivityDocument();
            args.Value.Hydrate(indexModel);
            await _elasticService.Index(indexModel);

            var docArgs = new PipelineArgs<ContentPostActivityDocument>()
            {
                Value = indexModel
            };
            await ContentPostActivityUtilities.SendWebsocketActivityUpdate(_messageHub, docArgs, "Create activity successful", DataRequestCompleteType.Created);
        }
        public async Task UpdatePostIndex(IPipelineArgs<ContentPostActivityEntity> args)
        {
            // fetch reviews
            var reviewsDoc = ContentPostActivityUtilities.GetActivities(_entityService, args);
            var fetchResponse = await _elasticPostService.GetDocument<ContentPostDocument>(args.Value.ContentPostId.ToString());
            var contentPostDocument = fetchResponse.Source;
            contentPostDocument.Reviews = reviewsDoc;
            contentPostDocument.UpdatedOn = DateTime.Now;
            await _elasticPostService.UpdateDocument(contentPostDocument, args.Value?.ContentPostId.ToString());

            // Send websocket request
            await ContentPostActivityUtilities.SendWebsocketPostUpdate(_messageHubPosts, args.Value?.UserId.ToString(), contentPostDocument, DataRequestCompleteType.Updated);
        } 
          

        // SYNC PROCESSORS
    }
}

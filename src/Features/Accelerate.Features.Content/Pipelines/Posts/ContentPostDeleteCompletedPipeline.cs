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
using Elastic.Clients.Elasticsearch.Ingest;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Accelerate.Features.Content.Pipelines.Posts
{
    public class ContentPostDeleteCompletedPipeline : DataDeleteCompletedEventPipeline<ContentPostEntity>
    {
        IHubContext<BaseHub<ContentPostEntity>, IBaseHubClient<WebsocketMessage<ContentPostEntity>>> _messageHub;
        public ContentPostDeleteCompletedPipeline(
            IHubContext<BaseHub<ContentPostEntity>, IBaseHubClient<WebsocketMessage<ContentPostEntity>>> messageHub)
        {
            _messageHub = messageHub;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostEntity>>()
            {
                //SendWebsocketUpdate
            };
            _processors = new List<PipelineProcessor<ContentPostEntity>>()
            {
            };
        }
        // ASYNC PROCESSORS
       
        
        // SYNC PROCESSORS
    }
}

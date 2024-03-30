using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Account.Services;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch.Ingest;
using Microsoft.AspNetCore.Identity;

namespace Accelerate.Features.Content.Pipelines
{
    public class AccountUserDeleteCompletedPipeline : DataDeleteCompletedEventPipeline<AccountUser>
    {
        public AccountUserDeleteCompletedPipeline()
        {
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<AccountUser>>()
            {
                SendWebsocketUpdate
            };
            _processors = new List<PipelineProcessor<AccountUser>>()
            {
            };
        }
        // ASYNC PROCESSORS
        public async Task SendWebsocketUpdate(IPipelineArgs<AccountUser> args)
        {
        }
        // SYNC PROCESSORS
    }
}

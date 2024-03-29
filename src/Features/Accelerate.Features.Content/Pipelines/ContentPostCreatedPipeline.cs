using Accelerate.Features.Content.Models.Data;
using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch.Ingest;
using Microsoft.AspNetCore.Identity;

namespace Accelerate.Features.Content.Pipelines
{
    public class ContentPostCreatedPipeline : DataCreateEventPipeline<ContentPostEntity>
    {
        IElasticService<ContentPostEntity> _elasticService;
        public ContentPostCreatedPipeline(
            IElasticService<ContentPostEntity> elasticService)
        {
            _elasticService = elasticService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostEntity>>()
            {
                IndexDocument
            };
            _processors = new List<PipelineProcessor<ContentPostEntity>>()
            {
            };
        }
        // ASYNC PROCESSORS
        public async Task IndexDocument(IPipelineArgs<ContentPostEntity> args)
        {
            var userId = args.Value.UserId.GetValueOrDefault().ToString();
            //var user = await _userManager.FindByIdAsync(userId);
            var indexModel = new ContentPost()
            {
                Status = args.Value.Status,
                Content = args.Value.Content,
                UserId = args.Value.UserId,
                CreatedOn = args.Value.CreatedOn,
                TargetThread = args.Value.TargetThread,
                ParentId = args.Value.ParentId,
                TargetChannel = args.Value.TargetChannel,
                TagItems = args.Value.TagItems,
                Category = args.Value.Category,
                Id = args.Value.Id,
                Username = args.Value.UserId.ToString() ?? "Anonymous"
            };
            await _elasticService.Index(indexModel);
        }
        // SYNC PROCESSORS
    }
}

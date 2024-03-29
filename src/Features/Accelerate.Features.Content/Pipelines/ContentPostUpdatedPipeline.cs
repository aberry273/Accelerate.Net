using Accelerate.Features.Content.Models.Data;
using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch.Ingest;

namespace Accelerate.Features.Content.Pipelines
{
    public class ContentPostUpdatedPipeline : DataUpdateEventPipeline<ContentPostEntity>
    {
        IElasticService<ContentPostEntity> _elasticService;
        public ContentPostUpdatedPipeline(
            IElasticService<ContentPostEntity> elasticService)
        {
            _elasticService = elasticService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostEntity>>()
            {
                UpdateDocument,
            };
        }
        // ASYNC PROCESSORS
        public async Task UpdateDocument(IPipelineArgs<ContentPostEntity> args)
        {
            var userId = args.Value.UserId.GetValueOrDefault().ToString();
            //var user = await _userManager.FindByIdAsync(userId);
            var indexModel = new ContentPost()
            {
                Id = args.Value.Id,
                TargetThread = args.Value.TargetThread,
                TargetChannel = args.Value.TargetChannel,
                Category = args.Value.Category,
                CreatedOn = args.Value.CreatedOn,
                UpdatedOn = args.Value.UpdatedOn,
                Tags = args.Value.Tags,
                ParentId = args.Value.ParentId,
                Content = args.Value.Content,
                Username = args.Value.UserId.ToString() ?? "Anonymous"
            };
            await _elasticService.UpdateDocument<ContentPostEntity>(indexModel, args.Value.Id.ToString());
        }
    }
}

using Accelerate.Features.Content.Models.Data;
using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Elastic.Clients.Elasticsearch.Ingest;

namespace Accelerate.Features.Content.Pipelines
{
    public class ContentPostCreatedPipeline : DataEventPipeline<ContentPostEntity>
    {
        IContentPostElasticService _contentElasticService;
        public ContentPostCreatedPipeline(
            IContentPostElasticService contentElasticService)
        {
            _contentElasticService = contentElasticService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostEntity>>()
            {
                AddToIndex
            };
            _processors = new List<PipelineProcessor<ContentPostEntity>>()
            {
            };
        }
        // ASYNC PROCESSORS
        public async Task AddToIndex(IPipelineArgs<ContentPostEntity> args)
        {
            var userId = args.Value.UserId.GetValueOrDefault().ToString();
            //var user = await _userManager.FindByIdAsync(userId);
            var indexModel = new ContentPost()
            {
                Content = args.Value.Content,
                User = args.Value.UserId.ToString() ?? "Anonymous"
            };
            var indexResponse = await _contentElasticService.Index(indexModel);
        }
        // SYNC PROCESSORS
    }
}

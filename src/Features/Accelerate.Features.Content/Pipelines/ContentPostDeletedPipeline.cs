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
    public class ContentPostDeletedPipeline : DataDeleteEventPipeline<ContentPostEntity>
    {
        IElasticService<ContentPostDocument> _elasticService;
        public ContentPostDeletedPipeline(
            IElasticService<ContentPostDocument> elasticService)
        {
            _elasticService = elasticService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostEntity>>()
            {
                DeleteDocument,
            };
        }
        // ASYNC PROCESSORS
        public async Task DeleteDocument(IPipelineArgs<ContentPostEntity> args)
        {
            var indexResponse = await _elasticService.DeleteDocument<ContentPostEntity>(args.Value.Id.ToString());
            var a = indexResponse;
        }
    }
}

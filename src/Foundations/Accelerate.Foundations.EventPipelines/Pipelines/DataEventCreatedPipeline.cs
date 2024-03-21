using Accelerate.Features.Content.Pipelines;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;


namespace Accelerate.Features.Content.Services
{
    public class DataEventCreatedPipeline<T> : BasePipelineProcessor<T>, IDataEventCreatedPipeline<T>
    {
        public DataEventCreatedPipeline()
        {

        }
        public DataEventCreatedPipeline(
            IEnumerable<PipelineProcessor<T>> processors,
            IEnumerable<AsyncPipelineProcessor<T>> asyncProcessors)
        {
            this._processors = processors;
            this._asyncProcessors = asyncProcessors;
        }
    }
}

using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;


namespace Accelerate.Foundations.EventPipelines.Pipelines
{
    public class DataCreateEventPipeline<T> : DataEventPipeline<T>, IDataCreateEventPipeline<T>
    {
    }
    public class DataUpdateEventPipeline<T> : DataEventPipeline<T>, IDataUpdateEventPipeline<T>
    {
    }
    public class DataDeleteEventPipeline<T> : DataEventPipeline<T>, IDataDeleteEventPipeline<T>
    {
    }
    public class DataEventPipeline<T> : BasePipelineProcessor<T>, IDataEventPipeline<T>
    {
        public DataEventPipeline()
        {

        }
        public DataEventPipeline(
            IEnumerable<PipelineProcessor<T>> processors,
            IEnumerable<AsyncPipelineProcessor<T>> asyncProcessors)
        {
            this._processors = processors;
            this._asyncProcessors = asyncProcessors;
        }
    }
}

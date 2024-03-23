using Accelerate.Features.Content.Consumers;
using Accelerate.Foundations.Common.Pipelines;

namespace Accelerate.Foundations.EventPipelines.Pipelines
{
    public interface IDataCreateEventPipeline<T> : IDataEventPipeline<T>
    {
    }
    public interface IDataUpdateEventPipeline<T> : IDataEventPipeline<T>
    {
    }
    public interface IDataDeleteEventPipeline<T> : IDataEventPipeline<T>
    {
    }
    public interface IDataEventPipeline<T> : IPipelineProcessor<T>
    {
    }
}

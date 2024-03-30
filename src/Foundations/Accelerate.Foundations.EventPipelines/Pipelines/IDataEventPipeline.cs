using Accelerate.Features.Content.Consumers;
using Accelerate.Foundations.Common.Pipelines;

namespace Accelerate.Foundations.EventPipelines.Pipelines
{
    public interface IDataCreateCompletedEventPipeline<T> : IDataEventPipeline<T>
    {
    }
    public interface IDataUpdateCompletedEventPipeline<T> : IDataEventPipeline<T>
    {
    }
    public interface IDataDeleteCompletedEventPipeline<T> : IDataEventPipeline<T>
    {
    }
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

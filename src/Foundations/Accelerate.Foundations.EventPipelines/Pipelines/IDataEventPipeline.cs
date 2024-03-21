using Accelerate.Features.Content.Consumers;
using Accelerate.Foundations.Common.Pipelines;

namespace Accelerate.Foundations.EventPipelines.Pipelines
{
    public interface IDataEventPipeline<T> : IPipelineProcessor<T>
    {
    }
}

using Accelerate.Features.Content.Consumers;
using Accelerate.Foundations.Common.Pipelines;

namespace Accelerate.Features.Content.Pipelines
{
    public interface IDataEventCreatedPipeline<T> : IPipelineProcessor<T>
    {
    }
}

using Accelerate.Features.Content.Consumers;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Content.Models;

namespace Accelerate.Features.Content.Pipelines
{
    public interface IContentPostCreatePipeline : IPipelineProcessor<ContentPostEntity>
    {
    }
}

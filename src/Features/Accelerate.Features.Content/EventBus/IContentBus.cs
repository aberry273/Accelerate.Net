using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.EventPipelines.EventBus;

namespace Accelerate.Features.Content.EventBus
{
    public interface IContentBus: IDataBus<ContentPostEntity>
    {
    }
}

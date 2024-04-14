using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.EventPipelines.EventBus;

namespace Accelerate.Features.Content.EventBus
{
    public interface IContentActivityBus: IDataBus<ContentPostActivityEntity>
    {
    }
}

using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.EventPipelines.EventBus;

namespace Accelerate.Foundations.Content.EventBus
{
    public interface IContentFeedBus: IDataBus<ContentFeedEntity>
    {
    }
}

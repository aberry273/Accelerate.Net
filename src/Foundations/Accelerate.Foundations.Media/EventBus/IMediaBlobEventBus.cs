using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.EventPipelines.EventBus;
using Accelerate.Foundations.Media.Models.Entities;

namespace Accelerate.Foundations.Media.EventBus
{
    public interface IMediaBlobEventBus : IDataBus<MediaBlobEntity>
    {
    }
}

using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.EventPipelines.EventBus;
using Accelerate.Foundations.Media.Models.Entities;

namespace Accelerate.Features.Content.EventBus
{
    public interface IMediaBlobEventBus : IDataBus<MediaBlobEntity>
    {
    }
}

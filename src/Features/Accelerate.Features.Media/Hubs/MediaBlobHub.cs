
using Accelerate.Foundations.Media.Models.Data;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using Microsoft.AspNetCore.SignalR;

namespace Accelerate.Features.Content.Hubs
{
    public class MediaBlobHub : BaseHub<MediaBlobDocument>
    {
    }
}

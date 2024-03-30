using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;

namespace Accelerate.Features.Content.Hubs
{
    public class ContentPostHub : BaseHub<ContentPostEntity>
    {
        public override async Task SendMessage(string user, WebsocketMessage<ContentPostEntity> data)
        {
            //await Clients.All.SendMessage("ReceiveMessage");
            await base.SendMessage(user, data);
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Websockets.Hubs
{
    public class BaseHub<T> : Hub<IBaseHubClient<T>>
    {
        public async Task SendMessage(string user, T data)
        {
            //await Clients.All.SendMessage("ReceiveMessage");
            await Clients.All.SendMessage(user, data);
        }
    }
}

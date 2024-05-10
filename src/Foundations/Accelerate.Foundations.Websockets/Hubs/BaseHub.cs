using Accelerate.Foundations.Websockets.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Websockets.Hubs
{
    public abstract class BaseHub<T> : Hub<IBaseHubClient<WebsocketMessage<T>>>
    {
        public virtual List<string> GetConnections(string user, WebsocketMessage<T> data)
        {
            // Message user(s) that own this
            var userConnections = HubClientConnectionsSingleton.GetUserConnections(user);
            var allConnections = new List<string>();
            allConnections.AddRange(userConnections);
            return allConnections.Distinct().ToList();
        }
        public virtual void UserRequest(string connectionId, string userId)
        {
            if(userId == null)
            {
                return;
            }
            //base.Groups.AddToGroupAsync(connectionId, userId);

            if (!HubClientConnectionsSingleton.UserConnections.ContainsKey(userId))
            {
                HubClientConnectionsSingleton.UserConnections.TryAdd(userId, new List<string>()
                {
                    connectionId
                });
            }
            else
            {
                var connections = HubClientConnectionsSingleton.UserConnections.FirstOrDefault(x => x.Key == userId).Value;
                if (!connections.Contains(connectionId))
                {
                    connections.Add(connectionId);
                }
            }
        }
        public virtual async Task SendMessage(string user, WebsocketMessage<T> data)
        {
            //await Clients.All.SendMessage("ReceiveMessage");
            var connectionIds = this.GetConnections(user, data);
            await Clients.Clients(connectionIds).SendMessage(user, data);
        }
    }
}

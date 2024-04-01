using Accelerate.Foundations.Websockets.Models;
using Microsoft.AspNetCore.SignalR;
using System;
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
        public abstract List<string> GetConnections(string user, WebsocketMessage<T> data);
        public virtual void UserRequest(string connectionId, string userId)
        {
            if(userId == null)
            {
                return;
            }
            //base.Groups.AddToGroupAsync(connectionId, userId);

            if (!HubClientConnectionsSingleton.UserConnections.ContainsKey(userId))
            {
                HubClientConnectionsSingleton.UserConnections.Add(userId, new List<string>()
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
        public virtual void ChannelRequest(string connectionId, string userId, string channelId)
        {
            if (channelId == null)
            {
                return;
            }
            //base.Groups.AddToGroupAsync(connectionId, channelId);

            if (!HubClientConnectionsSingleton.ChannelConnections.ContainsKey(channelId))
            {
                HubClientConnectionsSingleton.ChannelConnections.Add(channelId, new List<string>()
                {
                    connectionId
                });
            }
            else
            {
                var connections = HubClientConnectionsSingleton.ChannelConnections.FirstOrDefault(x => x.Key == channelId).Value;
                if (!connections.Contains(connectionId)){
                    connections.Add(connectionId);
                }
            }
        }
        public virtual void ThreadRequest(string connectionId, string userId, string threadId)
        {
            if (threadId == null)
            {
                return;
            }
            //base.Groups.AddToGroupAsync(connectionId, threadId);

            if (!HubClientConnectionsSingleton.ThreadConnections.ContainsKey(threadId))
            {
                HubClientConnectionsSingleton.ThreadConnections.Add(threadId, new List<string>()
                {
                    connectionId
                });
            }
            else
            {
                var connections = HubClientConnectionsSingleton.ThreadConnections.FirstOrDefault(x => x.Key == threadId).Value;
                if (!connections.Contains(connectionId)){
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

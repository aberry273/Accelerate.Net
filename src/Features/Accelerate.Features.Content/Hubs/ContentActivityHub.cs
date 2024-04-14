﻿using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using Microsoft.AspNetCore.SignalR;

namespace Accelerate.Features.Content.Hubs
{
    public class ContentActivityHub : BaseHub<ContentPostActivityDocument>
    {
        public override void UserRequest(string connectionId, string userId)
        {
            base.UserRequest(connectionId, userId);
        }
        public override void ChannelRequest(string connectionId, string userId, string channelId)
        {
            base.ChannelRequest(connectionId, userId, channelId);
        }
        public override void ThreadRequest(string connectionId, string userId, string threadId)
        {
            base.ThreadRequest(connectionId, userId, threadId);
        }
        public override List<string> GetConnections(string user, WebsocketMessage<ContentPostActivityDocument> message)
        { // Message user(s) that own this
            var userConnections = HubClientConnectionsSingleton.GetUserConnections(user);
            //var threadConnections = HubClientConnectionsSingleton.GetThreadConnections(message.Data?.TargetThread);
            //var channelConnections = HubClientConnectionsSingleton.GetChannelConnections(message.Data?.TargetChannel);
            var allConnections = new List<string>();
            allConnections.AddRange(userConnections);
            //allConnections.AddRange(threadConnections);
            //allConnections.AddRange(channelConnections);
            return allConnections.Distinct().ToList();
        }
    }
}

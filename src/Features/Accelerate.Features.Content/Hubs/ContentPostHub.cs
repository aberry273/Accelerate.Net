using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Twilio.TwiML.Messaging;

namespace Accelerate.Features.Content.Hubs
{
    public class ContentPostHub : BaseHub<ContentPostDocument>
    {
    }
}

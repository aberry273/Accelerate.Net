using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.Websockets.Hubs;
using Microsoft.AspNetCore.SignalR;
using Twilio.TwiML.Voice;

namespace Accelerate.Features.Content.Models.Contracts
{
    public class ContentPostCreateCompleteContract
    {
        public ContentPostCreateCompleteContract(ContentPostEntity entity)
        {
            Entity = entity;
        }
        public ContentPostEntity Entity { get; set; }
    }
}

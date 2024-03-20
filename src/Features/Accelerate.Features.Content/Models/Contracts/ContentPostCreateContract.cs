using Accelerate.Foundations.Content.Models;
using Twilio.TwiML.Voice;

namespace Accelerate.Features.Content.Models.Contracts
{
    public class ContentPostCreateContract
    {
        public ContentPostCreateContract(ContentPostEntity entity)
        {
            Entity = entity;
        }
        public ContentPostEntity Entity { get; set; }
    }
}

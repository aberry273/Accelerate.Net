using Accelerate.Foundations.Content.Models.Entities;

namespace Accelerate.Features.Content.Models.Data
{
    public class ContentPostQuoteRequest : ContentPostEntity
    {
        public List<string>? QuoteIds { get; set; }
    }
}

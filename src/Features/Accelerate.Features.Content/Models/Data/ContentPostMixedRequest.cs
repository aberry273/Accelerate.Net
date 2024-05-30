using Accelerate.Foundations.Content.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Accelerate.Features.Content.Models.Data
{
    public class ContentPostQuoteRequest
    {
        public Guid QuotedContentPostId { get; set; }
        public string? Content { get; set; }
        public string? Response { get; set; }
    }

    public class ContentPostMixedRequest : ContentPostEntity
    {
        public List<IFormFile>? Videos { get; set; }
        public List<IFormFile>? Images { get; set; }
        public List<Guid>? MediaIds { get; set; }
        public List<string>? QuotedItems { get; set; }
    }
}

using Accelerate.Foundations.Content.Models.Entities;

namespace Accelerate.Features.Content.Models.Data
{
    public class ContentPostMixedRequest : ContentPostEntity
    {
        public List<IFormFile>? Videos { get; set; }
        public List<IFormFile>? Images { get; set; }
        public List<Guid>? MediaIds { get; set; }
        public List<string>? QuoteIds { get; set; }
    }
}

using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Content.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Accelerate.Foundations.Content.Models.Data
{
    public class ContentPostLinkDocument
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public Guid ContentPostId { get; set; }
        public string ShortUrl { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}

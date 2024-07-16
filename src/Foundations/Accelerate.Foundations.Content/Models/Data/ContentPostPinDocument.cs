using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Integrations.Elastic.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Accelerate.Foundations.Content.Models.Data
{
    public class ContentPostPinDocument : EntityDocument
    {
        // Non-Indexed properties
        [JsonIgnore]
        [NotMapped]
        public ContentPostDocument? ContentPost { get; set; }

        [JsonIgnore]
        [NotMapped]
        public string Date { get; set; }
        // Indexed properties
        public Guid ContentPostId { get; set; }
        public Guid? UserId { get; set; }
        public string? Username { get; set; }
        public string? Reason { get; set; }
        public string? Href { get; set; }
    }
}

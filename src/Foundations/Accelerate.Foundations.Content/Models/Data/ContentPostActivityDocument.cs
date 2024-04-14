using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Content.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Accelerate.Foundations.Content.Models.Data
{
    public class ContentPostActivityDocument
    {
        // Core properties
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public Guid ContentPostId { get; set; }
        public Guid? UserId { get; set; }
        public ContentPostActivityTypes Type { get; set; }
        public string? Action { get; set; }
        public string? Value { get; set; }
    }
}

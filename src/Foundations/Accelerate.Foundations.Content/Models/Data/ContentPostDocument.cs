using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Content.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Accelerate.Foundations.Content.Models.Data
{
    public class ContentPostDocument
    {
        // Core properties
        public Guid? Id { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? ThreadId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? ParentId { get; set; }
        public ContentPostEntityStatus? Status { get; set; }
        public string? Content { get; set; }
        public string? TargetThread { get; set; }
        public string? TargetChannel { get; set; }
        public string? Category { get; set; }
        public IEnumerable<string>? Tags { get; set; }
        // Computed
        public string? Username { get; set; }
        public int? Agrees { get; set; }
        public int? Disagrees { get; set; }
        public int? Likes { get; set; }
        public int? Replies { get; set; }
    }
}

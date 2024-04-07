using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Content.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Accelerate.Foundations.Content.Models.Data
{
    public class ContentChannelDocument
    {
        // Core properties
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid? UserId { get; set; }
        public string? Category { get; set; }
        public IEnumerable<string>? Tags { get; set; }
    }
}

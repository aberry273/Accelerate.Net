using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Integrations.Elastic.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Accelerate.Foundations.Content.Models.Data
{
    public class ContentChannelDocument : EntityDocument
    {
        // Core properties 
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid? UserId { get; set; }
        public string? Category { get; set; }
        public IEnumerable<string>? Tags { get; set; }
    }
}

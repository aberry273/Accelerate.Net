using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Integrations.Elastic.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Accelerate.Foundations.Content.Models.Data
{
    public class ContentPostQuoteDocument : EntityDocument
    {
        // Core properties 
        public Guid? UserId { get; set; }
        public Guid ContentPostId { get; set; }
        public Guid QuotedContentPostId { get; set; }
        public string? Content { get; set; }
        public string? Response { get; set; }
    }
}

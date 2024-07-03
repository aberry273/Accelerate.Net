using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Integrations.Elastic.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Accelerate.Foundations.Content.Models.Data
{
    public class ContentPostMentionDocument : EntityDocument
    {
        // Core properties 
        public Guid ContentPostId { get; set; }
        public Guid UserId { get; set; }
    }
}

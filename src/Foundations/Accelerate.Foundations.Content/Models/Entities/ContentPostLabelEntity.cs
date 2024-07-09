using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Content.Models.Entities
{
    /// <summary>
    /// Join table for posts to posts
    /// </summary>
    [Table("ContentPostLabel")]
    public class ContentPostLabelEntity : BaseEntity
    {
        public string Label { get; set; }
        public string Reason { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }
        // Replying to another thread
        
        public ContentPostEntity ContentPost { get; set; }
        public Guid ContentPostId { get; set; } 
    }
}
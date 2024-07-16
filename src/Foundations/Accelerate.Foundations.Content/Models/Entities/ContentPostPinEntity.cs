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
    [Table("ContentPostPin")]
    public class ContentPostPinEntity : BaseEntity
    {
        public string Reason { get; set; }

        [ForeignKey("User")]
        public Guid UserId { get; set; }
        // Replying to another thread
        [NotMapped]
        public ContentPostEntity? ContentPost { get; set; }
        public Guid ContentPostId { get; set; }
        // Replying to another thread
        [NotMapped]
        public ContentPostEntity? PinnedContentPost { get; set; }
        public Guid PinnedContentPostId { get; set; }
    }
}
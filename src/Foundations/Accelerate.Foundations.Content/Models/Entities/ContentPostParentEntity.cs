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
    [Table("ContentPostParent")]
    public class ContentPostParentEntity : BaseEntity
    {
        public Guid ContentPostId { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        // Replying to another thread
        [NotMapped]
        public IEnumerable<Guid>? ParentIdItems
        {
            get
            {
                return ParentIds?.Split(',')?.Select(x => Guid.Parse(x)).ToList();
            }
            set
            {
                if (value != null) ParentIds = string.Join(',', value?.Select(x => x.ToString()));
            }
        }
        public string? ParentIds { get; set; }
        [NotMapped]
        public ContentPostEntity Parent { get; set; }
        public Guid? ParentId { get; set; } 
    }
}
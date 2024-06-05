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
    public enum ContentPostEntityStatus
    {
        Hidden, Public, Archived
    }
    [Table("ContentPosts")]
    public class ContentPostEntity : BaseEntity
    {
        [NotMapped]
        public string ThreadId => Id.ToBase64();

        [ForeignKey("User")]
        public Guid? UserId { get; set; }
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
        public Guid? ParentId { get; set; } 
        public ContentPostType Type { get; set; } = ContentPostType.Post;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ContentPostEntityStatus Status { get; set; } = ContentPostEntityStatus.Hidden;
        public string? Content { get; set; }
        // Which thread this message should be sent to
        /// <summary>
        /// OBSOLETE
        /// </summary>
        public string? TargetThread { get; set; }
        /// <summary>
        /// TO CHANGE TO JOIN TABLE
        /// </summary>
        // Which channel the user wants to send this message to
        public string? TargetChannel { get; set; }
        // For personal classification of the user
        public string? Category { get; set; }
        [NotMapped]
        public IEnumerable<string>? TagItems
        {
            get
            {
                return Tags?.Split(',')?.Select(x => x.Trim()).ToList();
            }
            set
            {
                if (value != null) Tags = string.Join(',', value?.Select(x => x?.Trim()));
            }
        }
        public string? Tags { get; set; }
        [NotMapped]
        public virtual ContentPostActionsSummaryEntity? Summary { get; set; }
        [NotMapped]
        public virtual ICollection<ContentPostQuoteEntity>? Quotes { get; set; }
        [NotMapped]
        public virtual ICollection<ContentPostActivityEntity>? Activities { get; set; }
        [NotMapped]
        public virtual ICollection<ContentPostActionsEntity>? Actions { get; set; }
        [NotMapped]
        public virtual ICollection<ContentPostMediaEntity>? Media { get; set; }
    }
}
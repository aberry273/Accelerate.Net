﻿using Accelerate.Foundations.Common.Extensions;
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
        public Guid? ParentId { get; set; } 
        public ContentPostType Type { get; set; } = ContentPostType.Post;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ContentPostEntityStatus Status { get; set; } = ContentPostEntityStatus.Hidden;
        public string? Content { get; set; }
        // Which thread this message should be sent to
        public string? TargetThread { get; set; }
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
        public virtual ICollection<ContentPostQuoteEntity>? Quotes { get; set; }
        [NotMapped]
        public virtual ICollection<ContentPostActivityEntity>? Activities { get; set; }
        [NotMapped]
        public virtual ICollection<ContentPostReviewEntity>? Reviews { get; set; }
        [NotMapped]
        public virtual ICollection<ContentPostReviewEntity>? Media { get; set; }
    }
}
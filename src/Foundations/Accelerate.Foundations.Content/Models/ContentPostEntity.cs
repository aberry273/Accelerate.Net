﻿using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Content.Models
{
    public enum ContentPostEntityStatus
    {
        Hidden, Public, Archived
    }
    [Table("ContentPosts")]
    public class ContentPostEntity : BaseEntity
    {
        [NotMapped]
        public string ThreadId => this.Id.ToBase64Clean();

        [ForeignKey("User")]
        public Guid? UserId { get; set; }
        // Replying to another thread
        public Guid? ParentId { get; set; }

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
                return this.Tags?.Split(',')?.Select(x => x.Trim()).ToList();
            }
            set
            {
                if (value != null) this.Tags = string.Join(',', value?.Select(x => x?.Trim()));
            }
        }
        public string? Tags { get; set; }
        public virtual ICollection<ContentPostActivityEntity>? Activities { get; set; }
        public virtual ICollection<ContentPostReviewEntity>? Reviews { get; set; }
    }
}
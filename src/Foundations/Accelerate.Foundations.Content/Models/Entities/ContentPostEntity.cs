using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Database.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Content.Models.Entities
{
    public enum ContentPostEntityStatus
    {
        Private, Public, Archived
    }
    [Table("ContentPosts")]
    public class ContentPostEntity : BaseEntity
    {
        [NotMapped]
        public string ThreadId => Id.ToBase64();

        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public ContentPostType Type { get; set; } = ContentPostType.Post;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ContentPostEntityStatus Status { get; set; } = ContentPostEntityStatus.Private;
        public string? ExternalId { get; set; }
        public string? Text { get; set; }
        public string? Formats { get; set; }
        [NotMapped]
        public List<ContentPostFormatItem>? FormatItems
        {
            get
            {
                if (string.IsNullOrEmpty(Formats)) return null;
                return System.Text.Json.JsonSerializer.Deserialize<List<ContentPostFormatItem>>(Formats);
            }
            set
            {
                Formats = System.Text.Json.JsonSerializer.Serialize(value);
            }
        }
        public List<string>? Tags { get; set; }
        [NotMapped]
        public virtual ContentPostTaxonomyEntity? Taxonomy { get; set; }
        [NotMapped]
        public virtual ContentPostSettingsEntity? Settings { get; set; }
        [NotMapped]
        public virtual ContentPostLinkEntity? Link { get; set; }
        [NotMapped]
        public virtual ContentPostActionsSummaryEntity? Summary { get; set; }
        [NotMapped]
        public virtual ICollection<ContentPostPinEntity>? Pins { get; set; }
        [NotMapped]
        public virtual ICollection<ContentPostMentionEntity>? Mentions { get; set; }
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
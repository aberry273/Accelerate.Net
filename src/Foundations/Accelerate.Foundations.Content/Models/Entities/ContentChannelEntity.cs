using Accelerate.Foundations.Common.Extensions;
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
    public enum ContentChannelEntityStatus
    {
        Hidden, Public, Archived
    }
    [Table("ContentChannels")]
    public class ContentChannelEntity : BaseEntity
    {

        [ForeignKey("User")]
        public Guid? UserId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ContentChannelEntityStatus Status { get; set; } = ContentChannelEntityStatus.Hidden;
        public string Name { get; set; }
        public string? Description { get; set; }
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
    }
}
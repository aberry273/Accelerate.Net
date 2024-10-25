using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Media.Models.Entities
{
    public enum MediaBlobFileType
    {
        Image, Video, File
    }
    public enum MediaBlobEntityStatus
    {
        Hidden, Public, Archived
    }
    [Table("MediaBlobs")]
    public class MediaBlobEntity : BaseEntity
    {

        [ForeignKey("User")]
        public Guid? UserId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MediaBlobFileType Type { get; set; } = MediaBlobFileType.File;
        public MediaBlobEntityStatus Status { get; set; } = MediaBlobEntityStatus.Hidden;
        public string Name { get; set; }
        public string? FilePath { get; set; }
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
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Models.Data;
using Accelerate.Foundations.Integrations.Elastic.Models;
using Accelerate.Foundations.Media.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Accelerate.Foundations.Media.Models.Data
{
    public class MediaBlobDocument : EntityDocument
    {
        // Core properties
        public string Name { get; set; }
        public string Image
        {
            get
            {
                return this.FilePath;
            }
        }
        public string Type { get; set; }
        public string FilePath { get; set; }
        public Guid? UserId { get; set; }
        public UserSubdocument User { get; set; }
        public IEnumerable<string>? Tags { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MediaBlobEntityStatus Status { get; set; } = MediaBlobEntityStatus.Hidden;
    }
}

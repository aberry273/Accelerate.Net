using Accelerate.Foundations.Database.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Accelerate.Foundations.Common.Extensions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Content.Models.Entities
{
    public class ContentPostSettingsFormat
    {
        public int Start { get; set; }
        public int End { get; set; }
        public string Value { get; set; }
        public string Format { get; set; }
    }
    [Table("ContentPostSettings")]
    public class ContentPostSettingsEntity : BaseEntity
    {
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        /// <summary>
        /// Which userGroup can respond
        /// </summary>
        public string Access { get; set; }
        /// <summary>
        /// How many responses each person can make
        /// </summary>
        public int PostLimit { get; set; }
        /// <summary>
        /// How many characters each response can have
        /// </summary>
        public int CharLimit { get; set; }
        /// <summary>
        /// How many images are allowed in each response
        /// </summary>
        public int ImageLimit { get; set; }
        /// <summary>
        /// How many videos are allowed in each response
        /// </summary>
        public int VideoLimit { get; set; }
        /// <summary>
        /// What text formatting is allowed in each response
        /// </summary>
        public string? Formats { get; set; }
        [NotMapped]
        public List<ContentPostSettingsFormat> FormatItems
        {
            get
            {
                if (string.IsNullOrEmpty(this.Formats)) return null;
                return Foundations.Common.Helpers.JsonSerializerHelper.SafelyDeserializeObject<List<ContentPostSettingsFormat>>(this.Formats);
            }
            set
            {
                if (value != null) this.Formats = Foundations.Common.Helpers.JsonSerializerHelper.SerializeMinimalObject(value);
            }
        }
        [NotMapped]
        public List<ContentPostSettingsPostEntity> ContentPosts { get; set; }
    }
}
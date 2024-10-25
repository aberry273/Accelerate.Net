using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Integrations.Elastic.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Accelerate.Foundations.Content.Models.Data
{
    public class ContentPostSettingsDocument : EntityDocument
    {
        /// <summary>
        /// Which userGroup can respond
        /// </summary>
        public string Access { get; set; }
        /// <summary>
        /// How many responses each person can make
        /// </summary>
        public int PostCount { get; set; }
        /// <summary>
        /// How many characters each response can have
        /// </summary>
        public int CharCount { get; set; }
        /// <summary>
        /// What text formatting is allowed in each response
        /// </summary>
        public List<ContentPostSettingsFormat> Formats { get; set; }
    }
}

using Accelerate.Foundations.Database.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Content.Models
{
    public enum ContentPostActivityTypes
    {
        //Many times to many channels
        Share,
        //Value once to post
        Vote
    }
    [Table("ContentPostActivity")]
    public class ContentPostActivityEntity : BaseEntity
    {
        [JsonIgnore]
        public virtual ContentPostEntity? ContentPost { get; set; }
        [ForeignKey("ContentPost")]
        public Guid ContentPostId { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public ContentPostActivityTypes Type { get; set; }
        /// <summary>
        /// Post replies set this as the root node
        /// </summary>
        public string Value { get; set; }
    }
}
using Accelerate.Foundations.Database.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Content.Models.Entities
{
    [Table("ContentPostActionsSummary")]
    public class ContentPostActionsSummaryEntity : BaseEntity
    {
        [JsonIgnore]
        public virtual ContentPostEntity? ContentPost { get; set; }
        [ForeignKey("ContentPost")]
        public Guid ContentPostId { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        /// <summary>
        /// Post replies set this as the root node
        /// </summary>
        public int Agrees { get; set; }
        public int Disagrees { get; set; }
        public int Quotes { get; set; }
        public int Replies { get; set; }
    }
}
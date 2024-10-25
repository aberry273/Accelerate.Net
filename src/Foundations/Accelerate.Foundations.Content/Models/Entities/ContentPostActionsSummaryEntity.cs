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
        /// <summary>
        /// Post replies set this as the root node
        /// </summary>
        public int Agrees { get; set; }
        public int Disagrees { get; set; }
        [NotMapped]
        public int? Votes
        {
            get
            {
                return this.Agrees - this.Disagrees;
            }
        }
        public int Quotes { get; set; }
        public int Replies { get; set; }
        public int Reactions { get; set; }
        [NotMapped]
        public IEnumerable<string>? ReactionItems
        {
            get
            {
                return TopReaction?.Split(',')?.Select(x => x.Trim()).ToList();
            }
            set
            {
                if (value != null) TopReaction = string.Join(',', value?.Select(x => x?.Trim()));
            }
        }
        public string? TopReaction { get; set; }
    }
}
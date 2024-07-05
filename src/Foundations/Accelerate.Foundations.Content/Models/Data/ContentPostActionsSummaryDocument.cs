using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Integrations.Elastic.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Accelerate.Foundations.Content.Models.Data
{
    public class ContentPostActionsSummaryDocument : EntityDocument
    {
        public Guid UserId { get; set; }
        public Guid ContentPostId { get; set; }
        public int Agrees { get; set; }
        public int Disagrees { get; set; }
        public int? Replies { get; set; }
        public int? Votes
        {
            get
            {
                return this.Agrees - this.Disagrees;
            }
        }
        public int? Quotes { get; set; }
        public int? Likes { get; set; }
        public int? Reactions { get; set; }
        public Dictionary<string, int> TopReactions { get; set; }
    }
}

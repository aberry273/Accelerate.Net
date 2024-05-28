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
    [Table("ContentPostQuotes")]
    public class ContentPostQuoteEntity : BaseEntity
    {
        public Guid QuotedContentPostId { get; set; }
        public Guid QuoterContentPostId { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public string Path { get; set; }
        [NotMapped]
        public int Level {
            get
            {
                if (string.IsNullOrWhiteSpace(Path)) return 0;
                return Path.Split(',').Length;
            }
        }
        public string? Value { get; set; }
        public string? Response { get; set; }
    }
}
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
        [NotMapped]
        public ContentPostEntity ContentPost { get; set; }
        public Guid ContentPostId { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public string? Content { get; set; }
        public string? Response { get; set; }
    }
}
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
    [Table("ContentPostMention")]
    public class ContentPostMentionEntity : BaseEntity
    {
        [NotMapped]
        public ContentPostEntity ContentPost { get; set; }
        public Guid ContentPostId { get; set; }
        public Guid UserId { get; set; }
    }
}
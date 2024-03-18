using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Content.Models
{
    [Table("ContentPosts")]
    public class ContentPostEntity : BaseEntity
    {
        [ForeignKey("User")]
        public Guid? UserId { get; set; }
        public string? Content { get; set; }
    }
}
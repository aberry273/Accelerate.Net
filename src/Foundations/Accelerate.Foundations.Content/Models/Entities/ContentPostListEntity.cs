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
    [Table("ContentPostList")]
    public class ContentPostListEntity : BaseEntity
    {
        public Guid ListId { get; set; }
        public Guid ContentPostId { get; set; }
        public int Order { get; set; }
    }
}
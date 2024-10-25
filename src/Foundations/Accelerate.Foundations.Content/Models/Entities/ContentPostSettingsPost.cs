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
    [Table("ContentPostSettingsPost")]
    public class ContentPostSettingsPostEntity : BaseEntity
    {
        [NotMapped]
        public ContentPostEntity ContentPost { get; set; }
        [ForeignKey("ContentPost")]
        public Guid ContentPostId { get; set; }
        [NotMapped]
        public ContentPostSettingsEntity ContentPostSettings { get;set;}
        [ForeignKey("ContentPostSettings")]
        public Guid ContentPostSettingsId { get; set; }
    }
}
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
    [Table("ContentPostLink")]
    public class ContentPostLinkEntity : BaseEntity
    {
        [NotMapped]
        public ContentPostEntity ContentPost { get; set; }
        public Guid ContentPostId { get; set; }
        public string ShortUrl
        {
            get
            {
                return this.Id.ToBase64Clean().Substring(0, 10);
            }
        }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string Url { get; set; }
    }
}
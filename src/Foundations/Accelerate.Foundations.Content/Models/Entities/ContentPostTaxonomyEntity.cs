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
    [Table("ContentPostTaxonomy")]
    public class ContentPostTaxonomyEntity : BaseEntity
    {
        [NotMapped]
        public ContentPostEntity ContentPost { get; set; }
        public Guid ContentPostId { get; set; }
        public string? Category { get; set; }
        [NotMapped]
        public IEnumerable<string>? TagItems
        {
            get
            {
                return Tags?.Split(',')?.Select(x => x.Trim()).ToList();
            }
            set
            {
                if (value != null) Tags = string.Join(',', value?.Select(x => x?.Trim()));
            }
        }
        public string? Tags { get; set; }
    }
}
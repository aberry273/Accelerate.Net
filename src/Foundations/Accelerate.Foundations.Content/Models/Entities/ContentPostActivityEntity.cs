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
    /*
    public static Dictionary<ContentPostActivityTypes, string> FiendlyTypeNames = new Dictionary<ContentPostActivityTypes, string>()
    {
        {
            ContentPostActivityTypes.Quote,
            "Quoted"
        }
    };
    */
    public enum ContentPostActivityTypes
    {
        Like,
        Agree,
        Disagree,
        // TODO
        Quote,
        // TODO
        Reply,
        Vote,
        Share,
        Mention,
        Created,
        Updated,
        Deleted,
    }
    [Table("ContentPostActivity")]
    public class ContentPostActivityEntity : BaseEntity
    {
        public Guid ContentPostId { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        [NotMapped]
        public string Action {
            get
            {
                return Enum.GetName(this.Type);
            }
            set
            {
                this.Type = Enum.Parse<ContentPostActivityTypes>(StringExtensions.Capitalize(value));
            }
        }
        public ContentPostActivityTypes Type { get; set; }
        public string? Value { get; set; }
        public string? Message { get; set; }
        public string? Url { get; set; }
    }
}
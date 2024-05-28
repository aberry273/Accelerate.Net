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
    public enum ContentPostActivityTypes
    {
        Like,
        // TODO
        Unlike,
        Agree,
        // TODO
        Unagree,
        Disagree,
        // TODO
        Undisagree,
        // TODO
        Quote,
        // TODO
        Reply,
        Vote,
        Share,
    }
    [Table("ContentPostActivity")]
    public class ContentPostActivityEntity : BaseEntity
    {
        [JsonIgnore]
        public virtual ContentPostEntity? ContentPost { get; set; }
        [ForeignKey("ContentPost")]
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
    }
}
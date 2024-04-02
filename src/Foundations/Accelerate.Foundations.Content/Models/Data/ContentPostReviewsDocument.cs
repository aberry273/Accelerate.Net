using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Content.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Accelerate.Foundations.Content.Models.Data
{
    public class ContentPostReviewsDocument
    {
        public int? Agrees { get; set; }
        public int? Disagrees { get; set; }
        public int? Likes { get; set; }
        public int? Replies { get; set; }
    }
}

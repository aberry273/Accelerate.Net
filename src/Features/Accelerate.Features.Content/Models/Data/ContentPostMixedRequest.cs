using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Accelerate.Features.Content.Models.Data
{
    public class ContentPostQuoteRequest
    {
        public Guid QuotedContentPostId { get; set; }
        public string? Content { get; set; }
        public string? Response { get; set; }
    }
    public class ContentPostLinkRequest
    {
        public string? Url { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
    } 
    public class ContentPostSettingsRequest
    {
        public string? Access { get; set; }
        public int? CharLimit { get; set; }
        public int? PostLimit { get; set; }
        public int? ImageLimit { get; set; }
        public int? VideoLimit { get; set; }
        public int? QuoteLimit { get; set; }
        //public List<ContentPostSettingsFormat> Formats { get; set; }
    }
    public class ContentPostMixedRequest : ContentPostEntity
    { 
        public string? LinkValue { get; set; }
        public Guid? ChannelId { get; set; }
        public IEnumerable<Guid>? ParentIdItems
        {
            get
            {
                return ParentIds?.Split(',')?.Select(x => Guid.Parse(x)).ToList();
            }
            set
            {
                if (value != null) ParentIds = string.Join(',', value?.Select(x => x.ToString()));
            }
        }
        public string? ParentIds { get; set; }
        public Guid? ParentId { get; set; }
        public List<IFormFile>? Videos { get; set; }
        public List<IFormFile>? Images { get; set; }
        public List<Guid>? MediaIds { get; set; }
        public List<string>? MentionItems { get; set; }
        public List<string>? QuotedItems { get; set; }
        //Taxonomy
        public List<string>? Tags { get; set; }
        public string? Category { get; set; }
        //Settings
        public int? CharLimit { get; set; }
        public int? WordLimit { get; set; }
        public int? VideoLimit { get; set; }
        public int? ImageLimit { get; set; }
        public int? QuoteLimit { get; set; }
        public string? Access { get; set; }
    }
}

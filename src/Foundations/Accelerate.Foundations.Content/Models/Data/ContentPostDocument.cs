using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Content.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Accelerate.Foundations.Content.Models.Data
{
    public enum ContentPostType
    {
        Post, Reply, Page
    }
    public class ContentPostMediaSubdocument
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string FilePath { get; set; }
        public string Type { get; set; }
    }
    public class ContentPostUserSubdocument
    {
        public string Username { get; set; }
        public string Image { get; set; }
    }
    public class ContentPostDocument
    {
        // Core properties
        public Guid Id { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string Href 
        { 
            get
            {
                return $"/Threads/{this.Id}";
            }
        }
        public string? ShortThreadId { get; set; }
        public string? ThreadId { get; set; }
        public Guid? UserId { get; set; }
        public List<Guid> ParentIds { get; set; }
        public Guid? ParentId { get; set; }
        public ContentPostEntityStatus? Status { get; set; }
        public string? Content { get; set; }
        public string? TargetThread { get; set; }
        public string? TargetChannel { get; set; }
        public string? ChannelName { get; set; }
        public string? Category { get; set; }
        public IEnumerable<string>? Tags { get; set; }
        public IEnumerable<string>? QuoteIds { get; set; }
        public IEnumerable<ContentPostMediaSubdocument>? Images { get; set; }
        // Computed
        public ContentPostReviewsDocument Reviews { get; set; }
        public ContentPostType PostType { get; set; } = ContentPostType.Post;
        //TODO: Replace with mapping 
        public List<Guid> ThreadIds { get; set; }
        public List<ContentPostDocument> Pages { get; set; }
        public ContentPostUserSubdocument? Profile { get; set; }
        public int? Quotes
        {
            get
            {
                if (Reviews == null) return 0;
                return Reviews.Quotes;
            }
        }
        public int? Agrees
        {
            get
            {
                if (Reviews == null) return 0;
                return Reviews.Agrees;
            }
        }
        public int? Disagrees
        {
            get
            {
                if (Reviews == null) return 0;
                return Reviews.Disagrees;
            }
        }
        public int? Likes
        {
            get
            {
                if (Reviews == null) return 0;
                return Reviews.Likes;
            }
        }
        public int? Replies { get; set; }
    }
}

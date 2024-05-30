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
    public class ContentPostQuoteSubdocument
    {
        public string ContentPostQuoteThreadId { get; set; }
        public string ContentPostQuoteId { get; set; }
        public string? Content { get; set; }
        public string? Response { get; set; }
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
        // Original posts threadId, all child threads reference the parent threadId which should become the original post
        public string? TargetThread { get; set; }
        public string? TargetChannel { get; set; }
        public string? ChannelName { get; set; }
        public string? Category { get; set; }
        public IEnumerable<string>? Tags { get; set; }
        public IEnumerable<ContentPostQuoteSubdocument>? QuotedPosts { get; set; }
        public IEnumerable<ContentPostMediaSubdocument>? Media { get; set; }
        // Computed
        public ContentPostActionsSummaryDocument ActionsTotals { get; set; }
        public ContentPostType PostType { get; set; } = ContentPostType.Post;
        //TODO: Replace with mapping 
        public List<Guid> ThreadIds { get; set; }
        public List<ContentPostDocument> Pages { get; set; }
        public ContentPostUserSubdocument? Profile { get; set; }
        public int? Quotes
        {
            get
            {
                if (ActionsTotals == null) return 0;
                return ActionsTotals.Quotes;
            }
        }
        public int? Agrees
        {
            get
            {
                if (ActionsTotals == null) return 0;
                return ActionsTotals.Agrees;
            }
        }
        public int? Disagrees
        {
            get
            {
                if (ActionsTotals == null) return 0;
                return ActionsTotals.Disagrees;
            }
        }
        public int? Likes
        {
            get
            {
                if (ActionsTotals == null) return 0;
                return ActionsTotals.Likes;
            }
        }
        public int? Replies { get; set; }
    }
}

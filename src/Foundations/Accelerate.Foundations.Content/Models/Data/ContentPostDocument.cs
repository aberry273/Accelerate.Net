using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Integrations.Elastic.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Accelerate.Foundations.Content.Models.Data
{
    public enum ContentPostType
    {
        Post, Reply, Page
    }
    public class ContentPostLinkSubdocument
    {
        public string? Url { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? ShortUrl { get; set; }
    }
    public class ContentPostQuoteSubdocument
    {
        public string ContentPostQuoteThreadId { get; set; }
        public string ContentPostQuoteId { get; set; }
        public string? Content { get; set; }
        public string? Response { get; set; }
    }
    public class ContentPostFormatSubdocument
    {
        public int Start { get; set; }
        public int End { get; set; }
        public string Value { get; set; }
        public string Format { get; set; }
    }
    public class ContentPostTaxonomySubdocument
    {
        public string? Category { get; set; }
        public List<string> Tags { get; set; }
    }
    public class ContentPostSettingsSubdocument
    {
        public ContentPostEntityStatus Status { get; set; }
        public string ContentPostSettingsId { get; set; }
        public string? Access { get; set; }
        public int? CharLimit { get; set; }
        public int? PostLimit { get; set; }
        public int? ImageLimit { get; set; }
        public int? VideoLimit { get; set; }
        public int? QuoteLimit { get; set; }
        public List<ContentPostSettingsFormat> Formats { get; set; }
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
    public class ContentPostDocument : EntityDocument
    {
        // Core properties
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
        public string? ParentVote { get; set; }
        // TODO: Update to a getter returning the settings.status
        public ContentPostEntityStatus? Status { get; set; }
        public string? Content { get; set; }
        
        public string? Date
        {
            get
            {
                return this.UpdatedOn.ToTimeSinceString();
            }
        }
        // Original posts threadId, all child threads reference the parent threadId which should become the original post
        
        public Guid? ChannelId { get; set; }
        public string? ChannelName { get; set; }
        // TODO: Update to a getter returning the settings.status
        public string? Category
        {
            get
            {
                return Taxonomy?.Category;
            }
        }
        public IEnumerable<string>? Tags 
        {
            get
            {
                return Taxonomy?.Tags;
            }
        }
        public IEnumerable<ContentPostQuoteSubdocument>? QuotedPosts { get; set; }
        public IEnumerable<ContentPostMediaSubdocument>? Media { get; set; }
        // Computed
        public ContentPostTaxonomySubdocument Taxonomy { get; set; }
        public ContentPostSettingsSubdocument Settings { get; set; }
        public ContentPostLinkSubdocument Link { get; set; }
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
        public int? TotalVotes
        {
            get
            {
                if (ActionsTotals == null) return 0;
                return ActionsTotals.Disagrees + ActionsTotals.Agrees;
            }
        }
        public int? Replies
        {
            get
            {
                if (ActionsTotals == null) return 0;
                return ActionsTotals.Replies;
            }
        }
    }
}

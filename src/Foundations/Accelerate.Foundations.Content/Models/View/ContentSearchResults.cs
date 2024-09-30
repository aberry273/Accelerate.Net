using Accelerate.Foundations.Content.Models.Data;

namespace Accelerate.Foundations.Content.Models.View
{
    public class ContentSearchResults
    {
        public IEnumerable<ContentPostViewDocument> Posts { get; set; } = new List<ContentPostViewDocument>();
        public IEnumerable<ContentPostDocument> QuotedPosts { get; set; } = new List<ContentPostDocument>();
        public IEnumerable<ContentPostActionsDocument> Actions { get; set; } = new List<ContentPostActionsDocument>();
        public IEnumerable<ContentPostActionsSummaryDocument> ActionSummaries { get; set; } = new List<ContentPostActionsSummaryDocument>();

    }
}

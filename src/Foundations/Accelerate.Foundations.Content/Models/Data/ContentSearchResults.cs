using Accelerate.Foundations.Content.Models.Data;

namespace Accelerate.Foundations.Content.Models.Data
{
    public class ContentSearchResults
    {
        public IEnumerable<ContentPostDocument> Posts { get; set; } = new List<ContentPostDocument>();
        public IEnumerable<ContentPostDocument> QuotedPosts { get; set; } = new List<ContentPostDocument>();
        public IEnumerable<ContentPostActionsDocument> Actions { get; set; } = new List<ContentPostActionsDocument>();
        public IEnumerable<ContentPostActionsSummaryDocument> ActionSummaries { get; set; } = new List<ContentPostActionsSummaryDocument>();

    }
}

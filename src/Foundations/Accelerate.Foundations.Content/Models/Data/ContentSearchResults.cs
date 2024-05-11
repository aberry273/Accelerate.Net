using Accelerate.Foundations.Content.Models.Data;

namespace Accelerate.Foundations.Content.Models.Data
{
    public class ContentSearchResults
    {
        public List<ContentPostDocument> Posts { get; set; } = new List<ContentPostDocument>();
        public List<ContentPostActionsDocument> Actions { get; set; } = new List<ContentPostActionsDocument>();
    }
}

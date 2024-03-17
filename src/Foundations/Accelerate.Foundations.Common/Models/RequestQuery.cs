namespace Accelerate.Foundations.Common.Models
{
    public class RequestQuery
    {
        public int Page { get; set; } = 0;
        public int ItemsPerPage { get; set; } = 10;
        public int Total { get; set; }
        public List<KeyValuePair<string, List<string>>>? Filters { get; set; }
        public string? Text { get; set; }
    }
}

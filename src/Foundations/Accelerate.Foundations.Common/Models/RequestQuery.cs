namespace Accelerate.Foundations.Common.Models
{
    public class RequestQuery<T> : RequestQuery
    {
        public T? Query { get; set; }
    }
    public class RequestQuery
    {
        public int Page { get; set; } = 0;
        public int ItemsPerPage { get; set; } = 10;
        public Dictionary<string, List<string>>? Filters { get; set; }
        public string? Text { get; set; }
    }
}

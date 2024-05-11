namespace Accelerate.Foundations.Common.Models
{
    public enum ElasticCondition
    {
        Must, MustNot, Should, Filter
    }
    public enum ValueType
    {
        String, Boolean, Long, Double, Null, False, True
    }
    public enum QueryOperator
    {
        Contains, Equals, NotEquals, Null, NotNull, GreaterThan, LessThan, Exist
    }
    public class QueryFilter
    {
        public QueryFilter(string name, QueryOperator @operator, ValueType valueType, List<object>? values, object? value)
        {
            Name = name;
            Operator = @operator;
            ValueType = valueType;
            Values = values;
            Value = value;
        }
        public QueryFilter()
        {
        }
        public ElasticCondition Condition { get; set; } = ElasticCondition.Must;
        public required string Name { get; set; }
        public ValueType ValueType { get; set; } = ValueType.String;
        public QueryOperator Operator { get; set; } = QueryOperator.Equals;
        public IEnumerable<object>? Values { get; set; }
        public object? Value { get; set; }
        public bool Keyword { get; set; } = true;
        public List<QueryFilter>? Filters { get; set; }
    }
    public class RequestQuery<T> : RequestQuery
    {
        public T? Query { get; set; }
    }
    public class RequestQuery
    {
        public Guid? UserId { get; set; }
        public int Page { get; set; } = 0;
        public int ItemsPerPage { get; set; } = 10;
        //public Dictionary<string, List<string>>? Filters { get; set; }
        public List<QueryFilter>? Filters { get; set; } = new List<QueryFilter>();
        public List<string>? Aggregates { get; set; } = new List<string>();
        public string? Text { get; set; }
    }
}

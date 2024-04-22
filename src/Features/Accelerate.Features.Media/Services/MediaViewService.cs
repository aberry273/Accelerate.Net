using Accelerate.Features.Content.Controllers;
using Accelerate.Features.Media.Services;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Helpers;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Media.Models.Data;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Aggregations;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace Accelerate.Features.Media.Services
{
    public class MediaViewService : IMediaViewService
    {
        IMetaContentService _metaContentService;
        public MediaViewService(IMetaContentService metaContent)
        {
            _metaContentService = metaContent;
        }
         
        public Dictionary<string, string> GetFilterOptions()
        {
            return new Dictionary<string, string>()
            {
            };
        }


        // NAVIGATION
        public List<NavigationFilter> CreateSearchFilters(SearchResponse<MediaBlobDocument> aggregateResponse)
        {
            var filterValues = new Dictionary<string, List<string>>();
            if (aggregateResponse.IsValidResponse)
            {
                var filterOptions = GetFilterOptions();
                filterValues = filterOptions.Values.ToDictionary(x => x, x => GetValuesFromAggregate(aggregateResponse.Aggregations, x));
            }
            var filter = new List<NavigationFilter>();
            return filter;
        }
        private List<string> GetValuesFromAggregate(AggregateDictionary aggregates, string key)
        {
            var agg = aggregates.FirstOrDefault(x => x.Key == key);
            StringTermsAggregate vals = agg.Value as StringTermsAggregate;
            if (vals == null || vals.Buckets == null || vals.Buckets.Count == 0) return new List<string>();

            var results = vals.Buckets.
                Select(x => x.Key.Value.ToString()).
                Where(x => !string.IsNullOrEmpty(x)).
                ToList();
            return results;
        }
        public List<QueryFilter> GetActualFilterKeys(List<QueryFilter>? Filters)
        {
            return Filters?.Select(x =>
            {
                x.Name = GetFilterKey(x.Name);
                return x;
            }).ToList();
        }
        public string GetFilterKey(string key)
        {
            var keyVal = this.GetFilterOptions().FirstOrDefault(x => x.Key == key);
            if (keyVal.Value == null) return key.ToCamelCase();
            return keyVal.Value?.ToCamelCase();
        }
        private List<string> GetAggregateValues(IDictionary<string, List<string>> aggFilters, string key)
        {
            if (key == null) return new List<string>();
            return aggFilters.ContainsKey(key) ? aggFilters[key] : new List<string>();
        } 

    }
}

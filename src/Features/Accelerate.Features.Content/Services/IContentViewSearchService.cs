using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Content.Models.Data;
using Elastic.Clients.Elasticsearch;
using Accelerate.Foundations.Content.Models.View;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Features.Content.Services
{
    public interface IContentViewSearchService
    {
        // Search methods
        Task<ContentSearchResults> SearchPosts(RequestQuery query, Guid postId);
        Task<ContentSearchResults> SearchUserPosts(RequestQuery query, Guid userId);
        Task<ContentSearchResults> SearchPostParents(Guid postId, RequestQuery query);
        Task<List<ContentPostPinDocument>> SearchPostPinned(Guid postId, RequestQuery query);
        Task<ContentSearchResults> SearchPostRepliesFromRoute(Guid postId, RequestQuery query);
        Task<ContentSearchResults> SearchPostReplies(RequestQuery query);
        Task<ContentSearchResults> SearchPosts(RequestQuery query);
        Task<ContentSearchResults> SearchPostsRelated(Guid channelId, RequestQuery query);
        Task<ContentSearchResults> SearchChannels(RequestQuery query);
        Task<ContentSearchResults> SearchFeedPosts(RequestQuery query);

        Task<List<ContentPostViewDocument>> UpdatePostDocuments(Guid? userId, List<ContentPostViewDocument> posts);
        Task<ContentPostViewDocument> UpdatePostDocument(Guid? userId, ContentPostDocument post);
        ContentPostViewDocument CreatePostViewModel(ContentPostDocument post);

        // Helper methods
        List<NavigationFilterValue> GetFilterSortOptions();
        string GetFilterKey(string key);
        List<QueryFilter> GetActualFilterKeys(List<QueryFilter>? Filters);
        string? GetSortField(List<QueryFilter>? Filters);
        SortOrder GetSortOrderField(List<QueryFilter>? Filters);
        List<NavigationFilterItem> CreateSearchFilters(SearchResponse<ContentPostDocument> aggregateResponse);
        List<KeyValuePair<string, string>> GetFilterOptions();
        NavigationFilter CreateNavigationFilters(SearchResponse<ContentPostDocument> aggregateResponse);
    }
}

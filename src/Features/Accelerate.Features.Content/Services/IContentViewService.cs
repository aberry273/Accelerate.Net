using Accelerate.Features.Content.Models.Views;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Elastic.Clients.Elasticsearch;

namespace Accelerate.Features.Content.Services
{
    public interface IContentViewService
    {
        NotFoundPage CreateNotFoundPage(AccountUser user, string title, string description);
        ChannelsPage CreateChannelsPage(AccountUser user, SearchResponse<ContentChannelDocument> channels, SearchResponse<ContentPostDocument> aggregateResponse);
        ChannelPage CreateChannelPage(AccountUser user, ContentChannelDocument item, SearchResponse<ContentChannelDocument> channels, SearchResponse<ContentPostDocument> aggregateResponse);
        ThreadPage CreateThreadPage(AccountUser user, ContentPostDocument item, SearchResponse<ContentPostDocument> aggregateResponse, ContentChannelDocument? channel = null);
        ThreadEditPage CreateEditThreadPage(AccountUser user, ContentPostDocument item, SearchResponse<ContentPostDocument> aggregateResponse, ContentChannelDocument? channel = null);
        ChannelsPage CreateAnonymousChannelsPage();
        List<NavigationFilterValue> GetFilterSortOptions();
        string GetFilterKey(string key);
        List<QueryFilter> GetActualFilterKeys(List<QueryFilter>? Filters);
        string? GetSortField(List<QueryFilter>? Filters);
        Elastic.Clients.Elasticsearch.SortOrder GetSortOrderField(List<QueryFilter>? Filters);
        public AjaxForm CreateChannelForm(AccountUser user);
        public ModalForm CreateModalChannelForm(AccountUser user);
        public ModalForm CreateModalEditReplyForm(AccountUser user);
        public AjaxForm CreateFormEditReply(AccountUser user);
        public ModalForm CreateModalDeleteReplyForm(AccountUser user);
        public AjaxForm CreateFormDeleteReply(AccountUser user);
        List<NavigationFilterItem> CreateSearchFilters(SearchResponse<ContentPostDocument> aggregateResponse);
        List<KeyValuePair<string, string>> GetFilterOptions();
        NavigationItem GetChannelLink(ContentChannelDocument x);
    }
}

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
        ThreadPage CreateThreadPage(AccountUser user, ContentPostDocument item, SearchResponse<ContentPostDocument> aggregateResponse, SearchResponse<ContentPostDocument> replies, ContentChannelDocument? channel = null);
        ChannelsPage CreateAnonymousChannelsPage();
        string GetFilterKey(string key);
        List<QueryFilter> GetActualFilterKeys(List<QueryFilter>? Filters);

        public AjaxForm CreatePostForm(AccountUser user, ContentChannelDocument channel = null);
        public AjaxForm CreateReplyForm(AccountUser user, ContentPostDocument post);
        public AjaxForm CreateChannelForm(AccountUser user);
        public ModalForm CreateModalChannelForm(AccountUser user);
        public ModalForm CreateModalEditReplyForm(AccountUser user);
        public AjaxForm CreateFormEditReply(AccountUser user);
        public ModalForm CreateModalDeleteReplyForm(AccountUser user);
        public AjaxForm CreateFormDeleteReply(AccountUser user);
        List<NavigationFilter> CreateSearchFilters(SearchResponse<ContentPostDocument> aggregateResponse);
        Dictionary<string, string> GetFilterOptions();
        public NavigationGroup GetChannelsDropdown(SearchResponse<ContentChannelDocument> searchResponse = null, string selectedName = null);
        NavigationItem GetChannelLink(ContentChannelDocument x);
    }
}

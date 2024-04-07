using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Elastic.Clients.Elasticsearch;

namespace Accelerate.Features.Content.Services
{
    public interface IContentViewService
    {
        public AjaxForm CreatePostForm(AccountUser user, ContentChannelDocument channel = null);
        public AjaxForm CreateReplyForm(AccountUser user, ContentPostDocument post);
        public AjaxForm CreateChannelForm(AccountUser user);
        public ModalForm CreateModalChannelForm(AccountUser user);
        public ModalForm CreateModalEditReplyForm(AccountUser user);
        public AjaxForm CreateFormEditReply(AccountUser user);
        public ModalForm CreateModalDeleteReplyForm(AccountUser user);
        public AjaxForm CreateFormDeleteReply(AccountUser user);
        List<NavigationFilter> CreateSearchFilters(SearchResponse<ContentPostDocument> aggregateResponse);
        List<string> GetFilterOptions();
        public NavigationGroup GetChannelsDropdown(string allChannelsUrl, SearchResponse<ContentChannelDocument> searchResponse = null, string selectedName = null);
        NavigationItem GetChannelLink(ContentChannelDocument x);
    }
}

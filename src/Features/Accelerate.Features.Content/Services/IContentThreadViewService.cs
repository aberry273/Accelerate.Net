using Accelerate.Features.Content.Models.UI;
using Accelerate.Features.Content.Models.Views;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Content.Models.View;
using Elastic.Clients.Elasticsearch;

namespace Accelerate.Features.Content.Services
{
    public interface IContentThreadViewService
    {
        ContentBasePage CreateAnonymousListingPage();
        NotFoundPage CreateNotFoundPage(AccountUser user, string title, string description);
        ContentPage CreateThreadsPage(AccountUser user, SearchResponse<ContentPostDocument> posts, SearchResponse<ContentPostDocument> aggregateResponse);
        ContentCreatePage CreateThreadCreatePage(AccountUser user, SearchResponse<ContentPostDocument> channels);
        ThreadPage CreateThreadPage(AccountUser user, ContentPostViewDocument item, SearchResponse<ContentPostDocument> aggregateResponse, ContentChannelDocument? channel = null);
        ContentCreatePage CreateThreadEditPage(AccountUser user, SearchResponse<ContentPostDocument> posts, ContentPostViewDocument post);
        ThreadEditPage CreateEditThreadPage(AccountUser user, ContentPostViewDocument item, SearchResponse<ContentPostDocument> aggregateResponse, ContentChannelDocument? channel = null);
        
    }
}

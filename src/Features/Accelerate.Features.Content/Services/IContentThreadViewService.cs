using Accelerate.Features.Content.Models.UI;
using Accelerate.Features.Content.Models.Views;
using Accelerate.Foundations.Users.Models.Entities;
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
        NotFoundPage CreateNotFoundPage(UsersUser user, string title, string description);
        ContentPage CreateThreadsPage(UsersUser user, SearchResponse<ContentPostDocument> posts, SearchResponse<ContentPostDocument> aggregateResponse);
        ContentCreatePage CreateThreadCreatePage(UsersUser user, SearchResponse<ContentPostDocument> channels);
        ContentPostPage CreateThreadPage(UsersUser user, ContentPostViewDocument item, SearchResponse<ContentPostDocument> aggregateResponse, ContentChannelDocument? channel = null);
        ContentCreatePage CreateThreadEditPage(UsersUser user, SearchResponse<ContentPostDocument> posts, ContentPostViewDocument post);
        ThreadEditPage CreateEditThreadPage(UsersUser user, ContentPostViewDocument item, SearchResponse<ContentPostDocument> aggregateResponse, ContentChannelDocument? channel = null);
        
    }
}

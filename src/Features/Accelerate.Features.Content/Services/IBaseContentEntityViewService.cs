using Accelerate.Features.Content.Models.UI;
using Accelerate.Features.Content.Models.Views;
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Content.Models.View;
using Accelerate.Foundations.Integrations.Elastic.Models;
using Elastic.Clients.Elasticsearch;

namespace Accelerate.Features.Content.Services
{
    public interface IBaseContentEntityViewService<T> where T : ContentEntityDocument
    {
        NotFoundPage CreateNotFoundPage(UsersUser user, string title = null, string description = null);
        ContentBasePage CreateAnonymousListingPage();
        Task<ContentBasePage> CreateAllPage(UsersUser user, SearchResponse<T> items, SearchResponse<ContentPostDocument> aggregateResponse);
        ContentBasePage CreateIndexPage(UsersUser user, SearchResponse<T> channels, SearchResponse<ContentPostDocument> aggregateResponse);
        Task<ContentBasePage> CreateEntityPage(UsersUser user, T item, SearchResponse<T> items, SearchResponse<ContentPostDocument> aggregateResponse);
        ContentCreatePage CreateAddPage(UsersUser user, SearchResponse<T> items);
        ContentCreatePage CreateEditPage(UsersUser user, SearchResponse<T> items, T item);
        AjaxForm CreateForm(UsersUser user, PostbackType type = PostbackType.POST, T? item = null);

        ContentSubmitForm CreatePostForm(UsersUser user, ContentPostViewDocument item = null, T doc = null);
    }
}

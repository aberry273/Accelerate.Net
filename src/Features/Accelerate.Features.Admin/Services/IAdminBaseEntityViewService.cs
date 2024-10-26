
using Accelerate.Features.Admin.Models.Views; 
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Content.Models.View;
using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.Integrations.Elastic.Models;
using Elastic.Clients.Elasticsearch;

namespace Accelerate.Features.Admin.Services
{
    public interface IAdminBaseEntityViewService<T> where T : IBaseEntity
    {
        NotFoundPage CreateNotFoundPage(UsersUser user, string title = null, string description = null);
        AdminBasePage CreateAnonymousListingPage();
        AdminBasePage CreateAllPage(UsersUser user, IEnumerable<T> items, SearchResponse<ContentPostDocument> aggregateResponse);
        AdminBasePage CreateIndexPage(UsersUser user, IEnumerable<T> channels, SearchResponse<ContentPostDocument> aggregateResponse);
        Task<AdminIndexPage<T>> CreateEntityPage(UsersUser user, T item, IEnumerable<T> items, SearchResponse<ContentPostDocument> aggregateResponse);
        AdminCreatePage CreateAddPage(UsersUser user, IEnumerable<T> items);
        AdminCreatePage CreateEditPage(UsersUser user, IEnumerable<T> items, T item);
        AjaxForm CreateEntityForm(UsersUser user, T? item, PostbackType type = PostbackType.POST);
    }
}

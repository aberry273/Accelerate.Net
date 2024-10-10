
using Accelerate.Features.Admin.Models.Views; 
using Accelerate.Foundations.Account.Models.Entities;
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
        NotFoundPage CreateNotFoundPage(AccountUser user, string title = null, string description = null);
        AdminBasePage CreateAnonymousListingPage();
        AdminBasePage CreateAllPage(AccountUser user, IEnumerable<T> items, SearchResponse<ContentPostDocument> aggregateResponse);
        AdminBasePage CreateIndexPage(AccountUser user, IEnumerable<T> channels, SearchResponse<ContentPostDocument> aggregateResponse);
        Task<AdminIndexPage<T>> CreateEntityPage(AccountUser user, T item, IEnumerable<T> items, SearchResponse<ContentPostDocument> aggregateResponse);
        AdminCreatePage CreateAddPage(AccountUser user, IEnumerable<T> items);
        AdminCreatePage CreateEditPage(AccountUser user, IEnumerable<T> items, T item);
        AjaxForm CreateEntityForm(AccountUser user, T? item, PostbackType type = PostbackType.POST);
    }
}

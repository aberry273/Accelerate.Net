
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.Integrations.Elastic.Models;
using Elastic.Clients.Elasticsearch;
using Accelerate.Features.Accounts.Models.Views; 

namespace Accelerate.Features.Accounts.Services
{
    public interface IAccountsBaseEntityViewService<T> where T : IBaseEntity
    {
        Task<AccountsNotFoundPage> CreateNotFoundPage(UsersUser user, string title = null, string description = null);
        Task<AccountsBasePage<T>> CreateListPage(UsersUser user, IEnumerable<T> items);
        Task<AccountsBasePage<T>> CreateIndexPage(UsersUser user);
        Task<AccountsBasePage<T>> CreateEntityPage(UsersUser user, T item);
        Task<AccountsFormPage> CreateNewPage(UsersUser user);
        Task<AccountsFormPage> CreateUpdatePage(UsersUser user, T item);
    }
}

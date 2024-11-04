
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.Integrations.Elastic.Models;
using Elastic.Clients.Elasticsearch;
using Accelerate.Features.Transactions.Models.Views; 

namespace Accelerate.Features.Transactions.Services
{
    public interface ITransactionsBaseEntityViewService<T> where T : IBaseEntity
    {
        Task<TransactionsNotFoundPage> CreateNotFoundPage(UsersUser user, string title = null, string description = null);
        Task<TransactionsBasePage<T>> CreateListPage(UsersUser user, IEnumerable<T> items);
        Task<TransactionsBasePage<T>> CreateIndexPage(UsersUser user);
        Task<TransactionsBasePage<T>> CreateEntityPage(UsersUser user, T item);
        Task<TransactionsFormPage> CreateNewPage(UsersUser user);
        Task<TransactionsFormPage> CreateUpdatePage(UsersUser user, T item);
    }
}

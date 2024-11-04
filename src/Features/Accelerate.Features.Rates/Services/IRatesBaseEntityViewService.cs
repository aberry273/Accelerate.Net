
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.Integrations.Elastic.Models;
using Elastic.Clients.Elasticsearch;
using Accelerate.Features.Rates.Models.Views; 

namespace Accelerate.Features.Rates.Services
{
    public interface IRatesBaseEntityViewService<T> where T : IBaseEntity
    {
        Task<RatesNotFoundPage> CreateNotFoundPage(UsersUser user, string title = null, string description = null);
        Task<RatesBasePage<T>> CreateListPage(UsersUser user, IEnumerable<T> items);
        Task<RatesBasePage<T>> CreateIndexPage(UsersUser user);
        Task<RatesBasePage<T>> CreateEntityPage(UsersUser user, T item);
        Task<RatesFormPage> CreateNewPage(UsersUser user);
        Task<RatesFormPage> CreateUpdatePage(UsersUser user, T item);
    }
}

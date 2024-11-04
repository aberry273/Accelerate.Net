
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.Integrations.Elastic.Models;
using Elastic.Clients.Elasticsearch;
using Accelerate.Features.Funding.Models.Views; 

namespace Accelerate.Features.Funding.Services
{
    public interface IFundingBaseEntityViewService<T> where T : IBaseEntity
    {
        Task<FundingNotFoundPage> CreateNotFoundPage(UsersUser user, string title = null, string description = null);
        Task<FundingBasePage<T>> CreateListPage(UsersUser user, IEnumerable<T> items);
        Task<FundingBasePage<T>> CreateIndexPage(UsersUser user);
        Task<FundingBasePage<T>> CreateEntityPage(UsersUser user, T item);
        Task<FundingFormPage> CreateNewPage(UsersUser user);
        Task<FundingFormPage> CreateUpdatePage(UsersUser user, T item);
    }
}

using Accelerate.Features.Admin.Models.Views;
using Accelerate.Foundations.Account.Models.Entities;

namespace Accelerate.Features.Admin.Services
{
    public interface IAdminViewService
    {
        AdminBasePage CreateJobsPage(AccountUser user);
    }
}

using Accelerate.Features.Admin.Models.Views;
using Accelerate.Foundations.Users.Models.Entities;

namespace Accelerate.Features.Admin.Services
{
    public interface IAdminViewService
    {
        AdminBasePage CreateJobsPage(UsersUser user);
    }
}

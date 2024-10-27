using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Projects.Api.Controllers
{
    [Route($"{Foundations.Common.Constants.ApiPaths.VersionPath}/accounts/customer")]
    [ApiController]
    public class AccountsCustomerController : BaseApiCommandController<AccountsCustomerEntity>
    {
        public AccountsCustomerController(IMediator mediator) : base(mediator)
        {
        }
    }
}

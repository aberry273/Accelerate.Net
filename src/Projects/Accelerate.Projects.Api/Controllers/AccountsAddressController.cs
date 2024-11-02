using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Projects.Api.Controllers
{
    [Route($"{Foundations.Common.Constants.ApiPaths.VersionPath}/accounts/address")]
    [ApiController]
    public class AccountsAddressController : BaseApiCommandController<AccountsAddressEntity>
    {
        public AccountsAddressController(IMediator mediator) : base(mediator)
        {
        }
    }
}

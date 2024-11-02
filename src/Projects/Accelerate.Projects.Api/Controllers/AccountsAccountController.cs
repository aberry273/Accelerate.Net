using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Users.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Projects.Api.Controllers
{
    [Route($"{Foundations.Common.Constants.ApiPaths.VersionPath}/accounts/account")]
    [ApiController]
    public class AccountsAccountController : BaseApiCommandController<AccountsBusinessEntity>
    {
        public AccountsAccountController(IMediator mediator) : base(mediator)
        {
        }
    }
}

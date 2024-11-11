using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Users.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Features.Api.Controllers
{
    [Route($"{Foundations.Common.Constants.ApiPaths.VersionPath}/accounts/business")]
    [ApiController]
    public class AccountsBusinessController : BaseApiCommandController<AccountsBusinessEntity>
    {
        public AccountsBusinessController(IMediator mediator) : base(mediator)
        {
        }
    }
}

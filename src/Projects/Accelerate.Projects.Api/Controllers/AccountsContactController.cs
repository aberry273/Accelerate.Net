using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Projects.Api.Controllers
{
    [Route($"{Foundations.Common.Constants.ApiPaths.VersionPath}/accounts/contact")]
    [ApiController]
    public class AccountsContactController : BaseApiCommandController<AccountsContactEntity>
    {
        public AccountsContactController(IMediator mediator) : base(mediator)
        {
        }
    }
}

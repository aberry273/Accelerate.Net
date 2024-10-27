using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Transfers.Models.Entities;
using Accelerate.Foundations.Users.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Projects.Api.Controllers
{
    [Route($"{Foundations.Common.Constants.ApiPaths.VersionPath}/users/user")]
    [ApiController]
    public class UsersUserController : BaseApiCommandController<UsersUser>
    {
        public UsersUserController(IMediator mediator) : base(mediator)
        {
        }
    }
}

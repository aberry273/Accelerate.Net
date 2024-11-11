
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Users.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Features.Api.Controllers
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

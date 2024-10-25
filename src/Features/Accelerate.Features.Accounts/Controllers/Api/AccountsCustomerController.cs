using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Features.Accounts.Controllers.Api
{
    [Route("api/v1/accounts/customer")]
    [ApiController]
    public class AccountsCustomerController : BaseApiCommandController<AccountsCustomerEntity>
    {
        public AccountsCustomerController(IMediator mediator) : base(mediator)
        {
        }
    }
}

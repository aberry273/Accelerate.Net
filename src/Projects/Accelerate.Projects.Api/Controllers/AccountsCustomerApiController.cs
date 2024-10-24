using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Accelerate.Projects.Api.Controllers
{
    public class AccountsCustomerApiController : BaseApiCommandController<AccountsCustomerEntity>
    {
        public AccountsCustomerApiController(IMediator mediator) : base(mediator)
        {
        }
    }
}
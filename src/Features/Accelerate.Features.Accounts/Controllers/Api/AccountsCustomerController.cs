using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using MediatR;

namespace Accelerate.Features.Accounts.Controllers.Api
{
    public class AccountsCustomerController : BaseApiCommandController<AccountsCustomerEntity>
    {
        public AccountsCustomerController(IMediator mediator) : base(mediator)
        {
        }
    }
}

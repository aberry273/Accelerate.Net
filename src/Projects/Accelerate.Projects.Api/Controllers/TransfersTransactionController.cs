using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Transfers.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Projects.Api.Controllers
{
    [Route($"{Foundations.Common.Constants.ApiPaths.VersionPath}/transfers/transaction")]
    [ApiController]
    public class TransfersTransactionController : BaseApiCommandController<TransfersTransactions>
    {
        public TransfersTransactionController(IMediator mediator) : base(mediator)
        {
        }
    }
}

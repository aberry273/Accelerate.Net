using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Transactions.Models.Entities;
using Accelerate.Foundations.Transactions.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Projects.Api.Controllers
{
    [Route($"{Foundations.Common.Constants.ApiPaths.VersionPath}/transactions/transaction")]
    [ApiController]
    public class TransactionsTransactionController : BaseApiCommandController<TransactionsTransactionEntity>
    {
        public TransactionsTransactionController(IMediator mediator) : base(mediator)
        {
        }
    }
}

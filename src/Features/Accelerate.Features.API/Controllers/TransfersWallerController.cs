using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Funding.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Features.Api.Controllers
{
    [Route($"{Foundations.Common.Constants.ApiPaths.VersionPath}/funding/wallet")]
    [ApiController]
    public class TransfersWalletController : BaseApiCommandController<FundingWalletEntity>
    {
        public TransfersWalletController(IMediator mediator) : base(mediator)
        {
        }
    }
}

using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Funding.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Features.Api.Controllers
{
    [Route($"{Foundations.Common.Constants.ApiPaths.VersionPath}/funding/bankaccount")]
    [ApiController]
    public class FundingBankAccountController : BaseApiCommandController<FundingBankAccountEntity>
    {
        public FundingBankAccountController(IMediator mediator) : base(mediator)
        {
        }
    }
}

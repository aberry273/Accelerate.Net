using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Rates.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Features.Api.Controllers
{
    [Route($"{Foundations.Common.Constants.ApiPaths.VersionPath}/rates/conversion/quote")]
    [ApiController]
    public class RatesConversionQuoteController : BaseApiCommandController<RatesConversionQuoteEntity>
    {
        public RatesConversionQuoteController(IMediator mediator) : base(mediator)
        {
        }
    }
}

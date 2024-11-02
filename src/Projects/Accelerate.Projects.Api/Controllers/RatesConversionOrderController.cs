using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Rates.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Projects.Api.Controllers
{
    [Route($"{Foundations.Common.Constants.ApiPaths.VersionPath}/rates/conversion/order")]
    [ApiController]
    public class RatesConversionOrderController : BaseApiCommandController<RatesConversionOrderEntity>
    {
        public RatesConversionOrderController(IMediator mediator) : base(mediator)
        {
        }
    }
}

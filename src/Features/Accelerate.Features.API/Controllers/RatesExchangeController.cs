using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.Mediator.Commands;
using Accelerate.Foundations.Mediator.Queries;
using Accelerate.Foundations.Rates.Models.Entities;
using AutoMapper.Configuration.Annotations;
using MassTransit.Mediator;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace Accelerate.Features.Api.Controllers
{
    public class RatesExchangeResponsePOCO : RatesExchangeRequestPOCO
    {
        public Guid Id { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public required decimal Rate { get; set; }
        public required DateTime ValidTo { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public override required string AccountId { get; set; }
    }
    public class RatesExchangeRequestPOCO : IBaseEntity
    {
        public virtual required string AccountId { get; set; }
        public required string FromCurrency { get; set; }
        public required string ToCurrency { get; set; }
        [Column(TypeName = "decimal(18,4)")] 
        public required DateTime LastRefreshed { get; set; }
        public required string TimeZone { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public Guid Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        [System.Text.Json.Serialization.JsonIgnore]
        public DateTime CreatedOn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        [System.Text.Json.Serialization.JsonIgnore]
        public DateTime? UpdatedOn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    } 
    [Route($"{Foundations.Common.Constants.ApiPaths.VersionPath}/rates/exchange")]
    [ApiController]
    public class RatesExchangeController : BaseApiCommandController<RatesExchangeRequestPOCO>
    {
        public RatesExchangeController(MediatR.IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        ///  Use this API to get the exchange rate between two currencies
        /// </summary>
    
        /// <response code="201">Returns the ID of the item created</response>
        /// <remarks>
        /// Sample request:
        ///     POST /
        ///     {
        ///        {
        ///             "AccountId": "AP0000001",
        ///             "FromCurrency": "INR",
        ///             "ToCurrency": "USDT",
        ///        },
        ///     }
        ///
        /// </remarks>
        /// 
        /// <response code="400">If an item could not be created</response>
        [HttpPost]
        [ProducesResponseType(typeof(RatesExchangeResponsePOCO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public override async Task<IActionResult> Post(RatesExchangeRequestPOCO entity)
        {
            return await base.Post(entity);
        }

    }
}

using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.Transactions.Models.Entities;
using Accelerate.Foundations.Transactions.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Accelerate.Features.Api.Controllers
{
    public class FundingSourceOnRampFIATPOCO : FundingSourceFIATPOCO
    {
    }
    public class TransferOnrampRequestPOCO : IBaseEntity
    {
        public bool AutoApproved { get; set; }
        public required string AccountId { get; set; }
        public required FundingSourceOnRampFIATPOCO Source { get; set; }
        public required FundingSourceWalletPOCO Destination { get; set; }
        public required RatesConversionPOCO Transaction { get; set; }
        [JsonIgnore]
        public Guid Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        [JsonIgnore] 
        public DateTime CreatedOn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        [JsonIgnore] 
        public DateTime? UpdatedOn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
    public class TransferOnrampResponsePOCO : IBaseEntity
    {
        public required string AccountId { get; set; }
        public required FundingSourceOnRampFIATPOCO Source { get; set; }
        public required FundingSourceWalletPOCO Destination { get; set; }
        public required RatesConversionPOCO Transaction { get; set; }
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        [JsonIgnore]
        public DateTime? UpdatedOn { get; set; }
    }

    [Route($"{Foundations.Common.Constants.ApiPaths.VersionPath}/transactions/onramp")]
    [ApiController]
    public class TransactionsOnrampController : BaseApiCommandController<TransferOnrampRequestPOCO>
    {
        public TransactionsOnrampController(IMediator mediator) : base(mediator)
        {
        }
        /// <summary>
        ///  Use this API to get create a transaction of money between a FIAT account and a crypto account. 
        /// </summary>

        /// <response code="201">Returns the ID of the item created</response>
        /// <remarks>
        /// Sample request:
        ///     POST /
        ///     {
        ///        {
        ///             "AccountId": "AP0000001",
        ///             "Source": {
        ///                 "BeneficiaryName": "PXXXXXXl",
        ///                 "AccountNumber": 37731372599"
        ///                 "BankName": "State bank of India",
        ///                 "IFSC": "SBIN0001041",
        ///                 "Email": "john@email.com"
        ///                 "Phone": "9283983239"
        ///             },
        ///             "Destination": {
        ///                 "Address": "0x742d35Cc6634C0532925a3b844Bc454e4438f44e"
        ///             },
        ///             "Transaction": {
        ///                 "FromCurrency": "USDT",
        ///                 "ToCurrency": "IDR",
        ///                 "Amount": 1200
        ///             },
        ///        },
        ///     }
        ///
        /// </remarks>
        /// 
        /// <response code="400">If an item could not be created</response>
        [HttpPost]
        [ProducesResponseType(typeof(TransferOnrampResponsePOCO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public override async Task<IActionResult> Post(TransferOnrampRequestPOCO entity)
        {
            return await base.Post(entity);
        }

        /// <summary>
        ///  Use this API to get approve of the OffRamp transaction between the FIAT account and crypto wallet. 
        /// </summary>
        [HttpPut]
        [Route("{id}/approve")]
        [ProducesResponseType(typeof(TransferOnrampResponsePOCO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<IActionResult> Approve([FromRoute] string id)
        {
            var entity = await base.Get(id);
            return entity;
        }
    }
}

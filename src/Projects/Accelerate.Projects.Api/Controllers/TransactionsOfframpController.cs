using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.Transactions.Models.Entities;
using Accelerate.Foundations.Transactions.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Twilio.TwiML.Voice;

namespace Accelerate.Projects.Api.Controllers
{
    public class AccountSourcePOCO
    {
        [Required]
        public required string CustomerId { get; set; }
    }
    public enum AccountTypePOCOEnum
    {
        NRO, NRE, Individual, Current
    }

    public class AccountCustomerPOCO
    {
        [Required]
        public required AccountTypePOCOEnum AccountType { get; set; }
        [Required]
        public required string Firstname { get; set; }
        [Required]
        public required string Lastname { get; set; }
        [Required]
        public required string DateOfBirth { get; set; }
        [Required]
        public required string Nationality { get; set; }
        [Required]
        public required KYCIdentificationPOCO Identification { get; set; }
        [Required]
        public required AccountAddressPOCO Address { get; set; }
    }
    public class KYCIdentificationPOCO
    {
        public string? PANCard { get; set; }
        public string? NationalId { get; set; }
    }
    public class AccountAddressPOCO
    {
        [Required]
        public string? StreetAddress1 { get; set; }
        public string? StreetAddress2 { get; set; }
        [Required]
        public string? Postcode { get; set; }
        public string? Suburb { get; set; }
        [Required]
        public string? City { get; set; }
        public string? Region { get; set; }
        [Required]
        public string? Country { get; set; }
    }
    public class RatesConversionPOCO
    {
        public required string FromCurrency { get; set; }
        public required string ToCurrency { get; set; }
        public required decimal Amount { get; set; }
        /// <summary>
        /// Optional - Use this in place of the currencies and amount to use a saved exchange rate to your account
        /// </summary>
        public Guid? ExchangeRateId { get; set; }
    }
    public class FundingSourceWalletPOCO
    {
        public required string Address { get; set; }
    }
    public class FundingSourceFIATPOCO
    {
        public required string BeneficiaryName { get; set; }
        public required int AccountNumber { get; set; }
        public required string BankName { get; set; }
        public required string IFSC { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
    }
    public class TransferOfframpRequestPOCO : IBaseEntity
    {
        public virtual required string AccountId { get; set; }
        public virtual required FundingSourceWalletPOCO Source { get; set; }
        public virtual required FundingSourceFIATPOCO Destination { get; set; }
        public virtual required RatesConversionPOCO Transaction { get; set; }
        public virtual required AccountCustomerPOCO Customer { get; set; }
        [JsonIgnore]
        public virtual Guid Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        [JsonIgnore] 
        public virtual DateTime CreatedOn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        [JsonIgnore] 
        public virtual DateTime? UpdatedOn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
    public enum TransactionStatus
    {
        InProgress, Approved, Successful, Failed
    }
    public class TransferOfframpResponsePOCO : TransferOfframpRequestPOCO
    {
        public bool AutoApproved { get; set; }
        public TransactionStatus Status { get; set; }
        public string Message { get; set; }
        public virtual Guid Id { get; set; }
        public virtual DateTime CreatedOn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        [JsonIgnore] 
        public virtual DateTime? UpdatedOn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }

    [Route($"{Foundations.Common.Constants.ApiPaths.VersionPath}/transactions/offramp")]
    [ApiController]
    public class TransactionsOfframpController : BaseApiCommandController<TransferOfframpRequestPOCO>
    {
        public TransactionsOfframpController(IMediator mediator) : base(mediator)
        {
        }
        /// <summary>
        ///  Use this API to get create a transaction of money between a Crypto wallet and a FIAT account. 
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
        [ProducesResponseType(typeof(TransferOfframpResponsePOCO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public override async Task<IActionResult> Post(TransferOfframpRequestPOCO entity)
        {
            return await base.Post(entity);
        }

        /// <summary>
        ///  Use this API to get approve of the OffRamp transaction between the Crypto wallet and a FIAT account. 
        /// </summary>
        [HttpPut]
        [Route("{id}/approve")]
        [ProducesResponseType(typeof(TransferOfframpResponsePOCO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<IActionResult> Approve([FromRoute] string id)
        {
            var entity = await base.Get(id);
            return entity;
        }
    }
}

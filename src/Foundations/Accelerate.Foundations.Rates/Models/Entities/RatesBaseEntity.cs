using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Rates.Models.Entities
{
    public enum RatesQuoteDatePreferenceEnum
    {
        Earliest, NextDay, Default, OptimizeLiquidity
    }
    public enum RatesQuoteFixedSideEnum
    {
        Buy, Sell
    }
    public class RatesBaseEntity : BaseEntity
    {
        public RatesCustomerEntity? Customer { get; set; }
        [ForeignKey("RatesCustomer")]
        public Guid? CustomerId { get; set; }
        [Required]
        public required Guid UserId { get; set; }
        public required string BuyCurrency { get; set; }
        public required string SellCurrency { get; set; }
        public decimal Amount { get; set; }
        public string? OnBehalfOf { get; set; }
        public RatesQuoteFixedSideEnum FixedSide { get; set; }
        public DateTime ConversionDate { get; set; }
        public RatesQuoteDatePreferenceEnum ConversionDatePreference { get; set; }
    }
}

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
        public required string ExternalId { get; set; }
        public RatesCustomerEntity? Customer { get; set; }
        [ForeignKey("RatesCustomer")]
        public Guid? CustomerId { get; set; }
        [Required]
        public required Guid UserId { get; set; }
        public required string BuyAsset { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public required decimal BuyPrice { get; set; }
        public required string SellAsset { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public required decimal SellPrice { get; set; }
        public decimal Volume { get; set; }
        public string? OnBehalfOf { get; set; }
        public RatesQuoteFixedSideEnum FixedSide { get; set; }
        public DateTime ConversionDate { get; set; }
        public RatesQuoteDatePreferenceEnum ConversionDatePreference { get; set; }
    }
}

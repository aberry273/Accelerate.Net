using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Rates.Models.Entities
{
    [Table("RatesConversionOrderEntity")]
    public class RatesConversionOrderEntity : RatesBaseEntity
    {
        #region Required 
        #endregion 
        public decimal BuyAmount { get; set; }
        public decimal SellAmount { get; set; }
        public string? Memo { get; set; }
        public string? UniqueId { get; set; }
        public string? TermsAgreement { get; set; }
    }
}

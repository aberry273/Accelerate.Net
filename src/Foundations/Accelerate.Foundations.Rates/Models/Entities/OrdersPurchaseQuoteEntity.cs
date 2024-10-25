using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Rates.Models.Entities
{
    [Table("RatesPurchaseQuote")]
    public class RatesPurchaseQuoteEntity : RatesBaseEntity
    {
        #region Required 
        #endregion 
        public RatesCustomerEntity? Customer { get; set; }
        [ForeignKey("RatesCustomer")]
        public Guid? CustomerId { get; set; }
    }
}

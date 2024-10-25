using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Orders.Models.Entities
{
    [Table("OrdersPurchaseQuote")]
    public class OrdersPurchaseQuoteEntity : OrdersBaseEntity
    {
        #region Required 
        #endregion 
        public OrdersCustomerEntity? Customer { get; set; }
        [ForeignKey("OrdersCustomer")]
        public Guid? CustomerId { get; set; }
    }
}

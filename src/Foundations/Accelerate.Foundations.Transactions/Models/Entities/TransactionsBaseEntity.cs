using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Transactions.Models.Entities
{
    public class TransactionsBaseEntity : BaseEntity
    {
        public required string Currency { get; set; } 
        public TransactionsCustomerEntity? Customer { get; set; }
        [ForeignKey("TransactionsCustomer")]
        public Guid? CustomerId { get; set; }
    }
}

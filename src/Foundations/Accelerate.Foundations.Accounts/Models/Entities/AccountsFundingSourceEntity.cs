using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Accounts.Models.Entities
{
    [Table("AccountsFundingSource")]
    public class AccountsFundingSourceEntity : AccountsBaseEntity
    {
        #region Required 
        #endregion 
        public AccountsBankAccountEntity? BankAccount { get; set; }
        [ForeignKey("AccountsCustomer")]
        public Guid? BankAccountId { get; set; }
        public AccountsCustomerEntity? Customer { get; set; }
        [ForeignKey("AccountsCustomer")]
        public Guid CustomerId { get; set; }
    }
}

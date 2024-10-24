using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Accounts.Models.Entities
{
    [Table("AccountsFunding")]
    public class AccountsBankAccountEntity : AccountsBaseEntity
    {
        #region Required 
        #endregion 
        [Required]
        public required string AccountOwnerName { get; set; }
        [Required]
        public required string RoutingNumber { get; set; }
        [Required]
        public required string AccountNumber { get; set; }
        [Required]
        public required string BankName { get; set; }
        [Required]
        public required string MaskedPan { get; set; }
        public AccountsCustomerEntity? Customer { get; set; }
        [ForeignKey("AccountsCustomer")]
        public Guid? CustomerId { get; set; }
    }
}

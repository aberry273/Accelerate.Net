using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Accounts.Models.Entities
{
    public enum BankAccountType
    {
        Savings, Debit
    }
    public enum AccountNumberType
    {
        IBAN, USLocal, Special
    }
    [Table("AccountsBankAccount")]
    public class AccountsBankAccountEntity : AccountsBaseEntity
    {
        public required BankAccountType AccountType { get; set; }
        public required AccountNumberType AccountNumberType { get; set; }
        #region Required 
        public required string AccountHolder { get; set; }
        public required string RoutingNumber { get; set; }
        public required string AccountNumber { get; set; }
        public required string BankName { get; set; }
        public required string MaskedPan { get; set; }
        public required string Country { get; set; }
        public required string Currency { get; set; }
        #endregion 
    }
}

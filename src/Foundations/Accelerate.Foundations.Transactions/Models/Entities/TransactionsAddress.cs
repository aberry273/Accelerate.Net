using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Transactions.Models.Entities
{
    public enum TransactionsAddressTypeEnum
    {
        Account, OneTimeAddress, Wallet
    }
    [Table("TransactionsAddress")]
    public class TransactionsAddressEntity : TransactionsBaseEntity
    {
        #region Required 
        #endregion
        public required TransactionsAddressTypeEnum Type { get; set; }
        public Guid? FundingSourceId { get; set; }
        public Guid? AccountsAccountId { get; set; }
        public Guid UserId { get; set; }
    }
}

using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Settlements.Models.Entities
{
    public enum AccountType
    {
        Asset, Liability, Revenue, Expense, Gain, Loss
    }
    [Table("SettlementsLedger")]
    public class SettlementsLedgerEntity : SettlementsBaseEntity
    {
        public required AccountType AccountType { get; set; }
        public required string Name { get; set; }
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

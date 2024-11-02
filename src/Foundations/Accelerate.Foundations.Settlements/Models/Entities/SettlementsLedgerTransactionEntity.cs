using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Settlements.Models.Entities
{
    [Table("SettlementsLedgerTransaction")]
    public class SettlementsLedgerTransactionEntity : SettlementsBaseEntity
    {
        public decimal Amount { get; set; }
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

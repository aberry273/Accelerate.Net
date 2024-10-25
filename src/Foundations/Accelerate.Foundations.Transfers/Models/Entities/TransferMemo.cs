using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Transfers.Models.Entities
{
    [Table("TransfersMemo")]
    public class TransfersMemoEntity : TransfersBaseEntity
    {
        #region Required 
        #endregion
        public string Memo { get; set; }
        public Guid? FundingSourceId { get; set; }
        public Guid? TransferPayinId { get; set; }
        public Guid? TransferPayoutId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid UserId { get; set; }
    }
}

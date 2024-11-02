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
    [Table("TransfersWallet")]
    public class TransfersWalletEntity : TransfersBaseEntity
    {
        #region Required 
        #endregion
        [NotMapped]
        public virtual List<TransfersCustomerEntity> Customer { get; set; }
        public Guid TransfersCustomerId { get; set; }
        public Guid UserId { get; set; }
        public required string Address { get; set; }
        public string? LegacyAddress { get; set; }
        public required string ExternalId { get; set; }
    }
}

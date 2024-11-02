using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Funding.Models.Entities
{
    [Table("FundingWallet")]
    public class FundingWalletEntity : FundingBaseEntity
    {
        #region Required 
        #endregion
        public Guid UserId { get; set; }
        public required string Address { get; set; }
        public string? LegacyAddress { get; set; }
        public required string ExternalId { get; set; }
    }
}

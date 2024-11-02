using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Funding.Models.Entities
{
    /// <summary>
    /// Join table for posts to posts
    /// </summary>
    [Table("FundingSource")]
    public class FundingSourceEntity : FundingBaseEntity
    {
        [NotMapped]
        public FundingWalletEntity? FundingVirtualAccount { get; set; }
        public Guid? FundingVirtualAccountId { get; set; }
        [NotMapped]
        public FundingWalletEntity? FundingWallet { get; set; }
        public Guid? FundingWalletId { get; set; }
        [NotMapped]
        public FundingBankAccountEntity? FundingBankAccount { get; set; }
        public Guid? FundingBankAccountId { get; set; }
    }
}
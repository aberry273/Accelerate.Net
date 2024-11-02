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
    [Table("FundingCustomer")]
    public class FundingCustomerEntity : BaseEntity
    {
        #region Required 
        #endregion
        public Guid? FundingSourceId { get; set; }
        public Guid? AccountsAccountId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid UserId { get; set; }
    }
}

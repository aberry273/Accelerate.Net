using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Kyc.Models.Entities
{
    [Table("KycCustomer")]
    public class KycCustomerEntity : BaseEntity
    {
        #region Required 
        #endregion
        public Guid? KycCheckIdentityId { get; set; }
        public Guid? KycCheckAmlCtfId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid UserId { get; set; }
    }
}

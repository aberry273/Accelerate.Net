using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Kyc.Models.Entities
{
    [Table("KycCheckIdentity")]
    public class KycCheckIdentityEntity : KycBaseEntity
    {
        #region Required 
        #endregion 
        public KycCustomerEntity? Customer { get; set; }
        [ForeignKey("KycCustomer")]
        public Guid? CustomerId { get; set; }
    }
}

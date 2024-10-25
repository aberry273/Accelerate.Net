using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Rates.Models.Entities
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public enum AccountCustomerType
    {
        Individual, Business
    }
    [Table("RatesCustomer")]
    public class RatesCustomerEntity : BaseEntity
    {
        #region Required 
        #endregion
        public Guid? FundingSourceId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid UserId { get; set; }
    }
}

using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Accounts.Models.Entities
{
    [Table("AccountsIndividual")]
    public class AccountsIndividualEntity : AccountsBaseCustomerEntity
    {
        #region Required 
        #endregion

        [NotMapped]
        public AccountsAddressEntity? BillingAddress { get; set; }
        public Guid BillingAddressId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Accounts.Models.Entities
{
    public class AccountsBaseCustomerEntity : AccountsContactEntity
    {
        public AccountsAddressEntity? Address { get; set; }
        public Guid? AddressId { get; set; }
        public Guid? KycIdentityId { get; set; }
        public Guid UserId { get; set; }
    }
}

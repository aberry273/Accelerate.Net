using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Accounts.Models.Entities
{
    /// <summary>
    /// Join table for posts to posts
    /// </summary>
    [Table("AccountsAccountAddress")]
    public class AccountsAccountAddressEntity : AccountsBaseEntity
    {
        [NotMapped]
        public AccountsBusinessEntity? AccountsAccount { get; set; }
        [ForeignKey("AccountsAccount")]
        public Guid AccountsAccountId { get; set; }
        [NotMapped] 
        public AccountsAddressEntity? AccountsAddress { get; set; }
        public Guid AccountsAddressId { get; set; }
    }
}
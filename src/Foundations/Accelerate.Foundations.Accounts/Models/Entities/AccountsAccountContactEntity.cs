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
    public enum AccountContactType
    {
        Owner, Shareholder, Reference, Other
    }
    /// <summary>
    /// Join table for accounts to contacts
    /// </summary>
    [Table("AccountsAccountContact")]
    public class AccountsAccountContactEntity : AccountsBaseEntity
    {
        public bool Primary { get; set; }
        public AccountContactType AccountContactType { get; set; }
        [NotMapped]
        public AccountsBusinessEntity? AccountsAccount { get; set; }
        [ForeignKey("AccountsAccount")]
        public Guid AccountsAccountId { get; set; }
        [NotMapped]
        public AccountsContactEntity? AccountsContact { get; set; }
        public Guid AccountsContactId { get; set; }
    }
}
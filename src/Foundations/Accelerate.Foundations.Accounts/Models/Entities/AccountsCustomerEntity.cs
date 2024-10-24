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
    // Add profile data for application users by adding properties to the ApplicationUser class
    public enum AccountCustomerType
    {
        Individual, Business
    }
    [Table("AccountsCustomer")]
    public class AccountsCustomerEntity : AccountsBaseEntity
    {
        #region Required 
        #endregion
        public AccountCustomerType Type { get; set; }
        [AllowNull]
        public string SignedAgreementId { get; set; }
        public string? Title { get; set; }
        [AllowNull]
        public string Email { get; set; }
        public string? EmailSecondary { get; set; }
        [AllowNull]
        public string Number { get; set; }
        public string? NumberSecondary { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? TaxId { get; set; }
        public string? DateOfBirth { get; set; }
        public Guid? KycIdentityId { get; set; }
        public Guid UserId { get; set; }
    }
}

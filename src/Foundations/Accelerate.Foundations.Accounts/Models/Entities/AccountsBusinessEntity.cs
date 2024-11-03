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
    public enum BusinessAccountType
    {
        SoleTrader, Partnership, Undefined, 
        BV, CVN, CVOA, EEN, MTS, NV, SE, VOF, 
        CCORP, TrustEstate, SoleProprietorship,
        SingleMemberLimitedLiability,
        LimitedLiability,
        PublicallyListedCompany,
        PrivateCompany,
        NonForProfit,
        NRO,
        NRE,
        Individual,
    }
    [Table("AccountsBusiness")]
    public class AccountsBusinessEntity : AccountsBaseEntity
    {
        #region Required 
        #endregion
        public BusinessAccountType Type { get; set; }
        public required string Name { get; set; }
        public string? Industry { get; set; }
        public string? RegistrationId { get; set; }
        public string? RegistrationAuthority { get; set; }
        public string? TaxId { get; set; }
        public string? Website { get; set; }

        [NotMapped]
        public AccountsAddressEntity? RegisteredAddress { get; set; }
        public Guid? RegisteredAddressId { get; set; }
        [NotMapped]
        public AccountsAddressEntity? OperatingAddress { get; set; }
        public Guid? OperatingAddressId { get; set; }

        [NotMapped]
        public AccountsAccountContactEntity? PrimaryContact { get; set; } 
        public Guid? PrimaryContactId { get; set; }

        [NotMapped]
        public virtual IEnumerable<AccountsAccountContactEntity> Contacts { get; set; }
        [NotMapped]
        public virtual IEnumerable<AccountsAccountChildEntity> ChildAccounts { get; set; }

        public string? SignedAgreementId { get; set; }
        //public Guid? KycIdentityId { get; set; }
        public Guid UserId { get; set; }
    }
}

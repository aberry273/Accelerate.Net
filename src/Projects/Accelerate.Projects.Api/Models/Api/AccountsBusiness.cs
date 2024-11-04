using Accelerate.Foundations.Accounts.Models;
using Accelerate.Foundations.Accounts.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Accelerate.Projects.Api.Models.Api
{
    public class AccountsBusinessRequestModel
    {
        public AccountsStatusEnum Status { get; set; }
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

    public class AccountsBusinessCreateRequestModel
    {

    }
    public class AccountsBusinessUpdateRequestModel
    {

    }
    public class AccountsBusinessResponseModel
    {

    }
}

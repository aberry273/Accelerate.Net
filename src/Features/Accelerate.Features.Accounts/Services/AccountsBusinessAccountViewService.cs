using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.UI.Components.Table;
using Accelerate.Foundations.Common.Services;
using Elastic.Clients.Elasticsearch;
using System.Collections.Generic;
using Accelerate.Foundations.Portal.Services;
using Accelerate.Features.Accounts.Services;
using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Features.Accounts.Models.Views;
using Accelerate.Foundations.Database.Services;
using MediatR;
using MassTransit.Mediator;
using Accelerate.Foundations.Mediator.Queries;
using System;

namespace Accelerate.Features.Admin.Services
{
    public class AccountsBusinessAccountViewService : AccountsBaseEntityViewService<AccountsBusinessEntity>
    {
        MediatR.IMediator _mediator;
        IEntityService<AccountsAddressEntity> _addressService;
        IMetaContentService _metaContentService;
        public AccountsBusinessAccountViewService(
            MediatR.IMediator mediator,
            IMetaContentService metaContent,
            IEntityService<AccountsAddressEntity> addressService,
            IPortalContentService portalContentService)
            : base(metaContent, portalContentService)
        {
            _mediator = mediator;
            _metaContentService = metaContent;
            _addressService = addressService;
            EntityName = "Business";
        }

        public override string GetEntityName(AccountsBusinessEntity item)
        {
            return item.Name;
        }
        /*
        private async Task<AccountsAddressEntity> GetAddress(Guid? guid)
        {
            return guid != null
                ? await _mediator.Send(new GetIdEntityQuery<AccountsAddressEntity>() { Id = guid })
                : null;
        }
        */
        public override async Task<AccountsBasePage<AccountsBusinessEntity>> CreateEntityPage(UsersUser user, AccountsBusinessEntity item)
        {
            var model = await base.CreateEntityPage(user, item);
            var viewModel = new AccountsBusinessAccountPage(model);

            //            var operatingAddress = _mediator.Send(Ge)
            
            
            var operatingAddress = item.OperatingAddressId != null
                ? _addressService.Get(item.OperatingAddressId.GetValueOrDefault())
                : null;
            if (operatingAddress != null)
            {
                viewModel.OperatingAddressForm = base.CreateEntityForm(user, item);
                viewModel.OperatingAddressForm.Fields = this.CreateAddressFormFields(user, operatingAddress);
            }

            var registeredAddress = item.OperatingAddressId != null
                ? _addressService.Get(item.OperatingAddressId.GetValueOrDefault())
                : null;
            if (registeredAddress != null)
            {
                viewModel.RegisteredAddressForm = base.CreateEntityForm(user, item);
                viewModel.RegisteredAddressForm.Fields = this.CreateAddressFormFields(user, registeredAddress);
            }
            viewModel.PrimaryContactForm = base.CreateAddressForm(user, item);
            viewModel.AccountForm = base.CreateEntityForm(user, item);
            viewModel.AccountForm.Fields = this.CreateAccountFormFields(user, item);
            
            return viewModel;
        }
        /*
         * public BusinessAccountType AccountType { get; set; }
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
        */
        public List<dynamic> BusinessAccountTypes()
        {
            return Enum.GetNames<Foundations.Accounts.Models.Entities.BusinessAccountType>().ToList<dynamic>();
        }
        public List<FormField> CreateAccountFormFields(UsersUser user, AccountsBusinessEntity? item)
        {
            var disabled = true;
            return new List<FormField>()
            {
                _metaContentService.FormField("Name", FormFieldComponents.aclFieldInput, null, null, item?.Name, disabled, false, null, null, null, "Business Name"),
                _metaContentService.FormFieldItems("AccountType", FormFieldComponents.aclFieldSelect, BusinessAccountTypes(), null, null, item?.AccountType, disabled, false, null, null, null, "Business Type"),
                _metaContentService.FormField("Industry", FormFieldComponents.aclFieldInput, null, null, item?.Industry, disabled, false, null, null, null, "Industry"),
                _metaContentService.FormField("RegistrationId", FormFieldComponents.aclFieldInput, null, null, item?.RegistrationId, disabled, false, null, null, null, "Business Registration ID"),
                _metaContentService.FormField("RegistrationAuthority", FormFieldComponents.aclFieldInput, null, null, item?.RegistrationAuthority, disabled, false, null, null, null, "Registration Authrotiy"),
                _metaContentService.FormField("TaxId", FormFieldComponents.aclFieldInput, null, null, item?.TaxId, disabled, false, null, null, null, "TaxId/VatId"),
                _metaContentService.FormField("Website", FormFieldComponents.aclFieldInput, null, null, item?.Website, disabled, false, null, null, null, "Website"),
                _metaContentService.FormField("Country", FormFieldComponents.aclFieldInput, null, null, item?.CountryCode, disabled, false, null, null, null, "Country"),
            };
        }
        public List<FormField> CreateAddressFormFields(UsersUser user, AccountsAddressEntity? item)
        {
            return new List<FormField>()
            {
                _metaContentService.FormField("StreetAddress1", FormFieldComponents.aclFieldInput, null, null, item.StreetAddress1, false, false, null, null, null, "Firstname"),
                _metaContentService.FormField("StreetAddress2", FormFieldComponents.aclFieldInput, null, null, item.StreetAddress2, false, false, null, null, null, "Lastname"),
                _metaContentService.FormField("Postcode", FormFieldComponents.aclFieldInput, null, null, item.Postcode, false, false, null, null, null, "Email"),
                _metaContentService.FormField("Suburb", FormFieldComponents.aclFieldInput, null, null, item.Suburb, false, false, null, null, null, "Email"),
                _metaContentService.FormField("City", FormFieldComponents.aclFieldInput, null, null, item.City, false, false, null, null, null, "Email"),
                _metaContentService.FormField("Region", FormFieldComponents.aclFieldInput, null, null, item.Region, false, false, null, null, null, "Email"),
                _metaContentService.FormField("Country", FormFieldComponents.aclFieldInput, null, null, item.Country, false, false, null, null, null, "Email"),
            };
        }
    }
}

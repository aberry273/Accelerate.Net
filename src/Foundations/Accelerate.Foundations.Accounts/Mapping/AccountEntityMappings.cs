using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.Mediator.Commands;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Mediator.Mappings
{
    public class AddressBusinessMapping : EntityMapping<AccountsBusinessEntity> { }
    public class AddressIndividualMapping : EntityMapping<AccountsIndividualEntity> { }
    public class AddressEntityMapping : EntityMapping<AccountsAddressEntity> { }
}

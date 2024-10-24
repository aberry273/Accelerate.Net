using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Mediator.Commands;
using Accelerate.Foundations.Mediator.Queries;
using AutoMapper;
using MassTransit.Futures.Contracts;

namespace Accelerate.Features.Accounts.Commands
{
    public class AccountsCustomerCommands
    {
        public class CreateAccountsCustomerCommand : CreateEntityCommand<AccountsCustomerEntity>;
        public class UpdateAccountsCustomerCommand : UpdateEntityCommand<AccountsCustomerEntity>;
        public class DeleteAccountsCustomerCommand : DeleteEntityCommand<AccountsCustomerEntity>;
        public class CreateAccountsCustomerHandler : CreateEntityHandler<AccountsCustomerEntity>
        {
            public CreateAccountsCustomerHandler(IEntityService<AccountsCustomerEntity> service, IMapper mapper) : base(service, mapper)
            {

            }
        }
        public class UpdateAccountsCustomerHandler : UpdateEntityHandler<AccountsCustomerEntity>
        {
            public UpdateAccountsCustomerHandler(IEntityService<AccountsCustomerEntity> service, IMapper mapper) : base(service, mapper)
            {

            }
        }
        public class DeleteAccountsCustomerHandler : DeleteEntityHandler<AccountsCustomerEntity>
        {
            public DeleteAccountsCustomerHandler(IEntityService<AccountsCustomerEntity> service, IMapper mapper) : base(service, mapper)
            {

            }
        }
        public class FindEntityAccountsCustomerHandler : FindEntityHandler<AccountsCustomerEntity>
        {
            public FindEntityAccountsCustomerHandler(IEntityService<AccountsCustomerEntity> service, IMapper mapper) : base(service, mapper)
            {

            }
        }
        public class FindByIdAccountsCustomerHandler : GetByIdEntityHandler<AccountsCustomerEntity>
        {
            public FindByIdAccountsCustomerHandler(IEntityService<AccountsCustomerEntity> service, IMapper mapper) : base(service, mapper)
            {

            }
        }
    }
}

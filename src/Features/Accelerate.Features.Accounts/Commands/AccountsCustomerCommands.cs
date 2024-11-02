using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Mediator.Commands;
using Accelerate.Foundations.Mediator.Queries;
using AutoMapper;
using MassTransit.Futures.Contracts;
using MediatR;

namespace Accelerate.Features.Accounts.Commands
{
    public class AccountsCustomerCommands
    {
        public class CreateAccountsCustomerCommand : CreateEntityCommand<AccountsBusinessEntity>;
        public class UpdateAccountsCustomerCommand : UpdateEntityCommand<AccountsBusinessEntity>;
        public class DeleteAccountsCustomerCommand : DeleteEntityCommand<AccountsBusinessEntity>;
      
        public class CreateAccountsCustomerHandler : CreateEntityHandler<AccountsBusinessEntity>
        {
            public CreateAccountsCustomerHandler(BaseContext<AccountsBusinessEntity> context, IMapper mapper, IMediator mediator) : base(context, mapper, mediator) { }
        }
       
        public class UpdateAccountsCustomerHandler : UpdateEntityHandler<AccountsBusinessEntity>
        {
            public UpdateAccountsCustomerHandler(BaseContext<AccountsBusinessEntity> context, IMapper mapper, IMediator mediator) : base(context, mapper, mediator) { }
        }
        public class DeleteAccountsCustomerHandler : DeleteEntityHandler<AccountsBusinessEntity>
        {
            public DeleteAccountsCustomerHandler(BaseContext<AccountsBusinessEntity> context, IMapper mapper, IMediator mediator) : base(context, mapper, mediator) { }
        }
        public class FindEntityAccountsCustomerHandler : FindEntityHandler<AccountsBusinessEntity>
        {
            public FindEntityAccountsCustomerHandler(IEntityService<AccountsBusinessEntity> service, IMapper mapper) : base(service, mapper) { }
        }
        public class FindByIdAccountsCustomerHandler : GetByIdEntityHandler<AccountsBusinessEntity>
        {
            public FindByIdAccountsCustomerHandler(IEntityService<AccountsBusinessEntity> service, IMapper mapper) : base(service, mapper) { }
        }
    }
}

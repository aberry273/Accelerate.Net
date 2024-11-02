using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Mediator.Commands;
using Accelerate.Foundations.Mediator.Queries;
using AutoMapper;
using MassTransit.Futures.Contracts;
using MediatR;

namespace Accelerate.Foundations.Accounts.Commands
{
    public class AccountsBusinessCommands
    {
        public class CreateAccountsBusinessCommand : CreateEntityCommand<AccountsBusinessEntity>;
        public class UpdateAccountsBusinessCommand : UpdateEntityCommand<AccountsBusinessEntity>;
        public class DeleteAccountsBusinessCommand : DeleteEntityCommand<AccountsBusinessEntity>;
        public class CreateAccountsBusinessHandler : CreateEntityHandler<AccountsBusinessEntity> {
            public CreateAccountsBusinessHandler(BaseContext<AccountsBusinessEntity> service, IMapper mapper, IMediator mediator) : base(service, mapper, mediator) { }
        }
        public class UpdateAccountsBusinessHandler : UpdateEntityHandler<AccountsBusinessEntity> {
            public UpdateAccountsBusinessHandler(BaseContext<AccountsBusinessEntity> service, IMapper mapper, IMediator mediator) : base(service, mapper, mediator) { }
        }
        public class DeleteAccountsBusinessHandler : DeleteEntityHandler<AccountsBusinessEntity> {
            public DeleteAccountsBusinessHandler(BaseContext<AccountsBusinessEntity> service, IMapper mapper, IMediator mediator) : base(service, mapper, mediator) { }
        }
        public class FindEntityAccountsBusinessHandler : FindEntityHandler<AccountsBusinessEntity> {
            public FindEntityAccountsBusinessHandler(IEntityService<AccountsBusinessEntity> service, IMapper mapper) : base(service, mapper) { }
        }
        public class FindByIdAccountsBusinessHandler : GetByIdEntityHandler<AccountsBusinessEntity> {
            public FindByIdAccountsBusinessHandler(IEntityService<AccountsBusinessEntity> service, IMapper mapper) : base(service, mapper) { }
        }
    }
}

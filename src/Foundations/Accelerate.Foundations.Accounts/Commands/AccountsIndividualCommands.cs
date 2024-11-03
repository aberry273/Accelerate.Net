using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Mediator.Commands;
using Accelerate.Foundations.Mediator.Queries;
using AutoMapper;
using MassTransit.Futures.Contracts;
using MediatR;

namespace Accelerate.Foundations.Accounts.Commands
{
    public class AccountsIndividualCommands
    {
        public class CreateAccountsIndividualCommand : CreateEntityCommand<AccountsIndividualEntity>;
        public class UpdateAccountsIndividualCommand : UpdateEntityCommand<AccountsIndividualEntity>;
        public class DeleteAccountsIndividualCommand : DeleteEntityCommand<AccountsIndividualEntity>;
        public class CreateAccountsIndividualHandler : CreateEntityHandler<AccountsIndividualEntity> {
            public CreateAccountsIndividualHandler(BaseContext<AccountsIndividualEntity> service, IMapper mapper, IMediator mediator) : base(service, mapper, mediator) { }
        }
        public class UpdateAccountsIndividualHandler : UpdateEntityHandler<AccountsIndividualEntity> {
            public UpdateAccountsIndividualHandler(BaseContext<AccountsIndividualEntity> service, IMapper mapper, IMediator mediator) : base(service, mapper, mediator) { }
        }
        public class DeleteAccountsIndividualHandler : DeleteEntityHandler<AccountsIndividualEntity> {
            public DeleteAccountsIndividualHandler(BaseContext<AccountsIndividualEntity> service, IMapper mapper, IMediator mediator) : base(service, mapper, mediator) { }
        }
        public class FindEntityAccountsIndividualHandler : FindEntityHandler<AccountsIndividualEntity> {
            public FindEntityAccountsIndividualHandler(IEntityService<AccountsIndividualEntity> service, IMapper mapper) : base(service, mapper) { }
        }
        public class FindByIdAccountsIndividualHandler : GetByIdEntityHandler<AccountsIndividualEntity> {
            public FindByIdAccountsIndividualHandler(IEntityService<AccountsIndividualEntity> service, IMapper mapper) : base(service, mapper) { }
        }
    }
}

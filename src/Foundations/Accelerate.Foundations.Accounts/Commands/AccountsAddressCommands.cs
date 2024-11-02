using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Mediator.Commands;
using Accelerate.Foundations.Mediator.Queries;
using AutoMapper;
using MassTransit.Futures.Contracts;
using MediatR;

namespace Accelerate.Foundations.Accounts.Commands
{
    public class AccountsAddressCommands
    {
        public class CreateAddressCustomerCommand : CreateEntityCommand<AccountsAddressEntity>;
        public class UpdateAddressCustomerCommand : UpdateEntityCommand<AccountsAddressEntity>;
        public class DeleteAddressCustomerCommand : DeleteEntityCommand<AccountsAddressEntity>;
        public class CreateAddressCustomerHandler : CreateEntityHandler<AccountsAddressEntity> {
            public CreateAddressCustomerHandler(BaseContext<AccountsAddressEntity> service, IMapper mapper, IMediator mediator) : base(service, mapper, mediator) { }
        }
        public class UpdateAddressCustomerHandler : UpdateEntityHandler<AccountsAddressEntity> {
            public UpdateAddressCustomerHandler(BaseContext<AccountsAddressEntity> service, IMapper mapper, IMediator mediator) : base(service, mapper, mediator) { }
        }
        public class DeleteAddressCustomerHandler : DeleteEntityHandler<AccountsAddressEntity> {
            public DeleteAddressCustomerHandler(BaseContext<AccountsAddressEntity> service, IMapper mapper, IMediator mediator) : base(service, mapper, mediator) { }
        }
        public class FindEntityAddressCustomerHandler : FindEntityHandler<AccountsAddressEntity> {
            public FindEntityAddressCustomerHandler(IEntityService<AccountsAddressEntity> service, IMapper mapper) : base(service, mapper) { }
        }
        public class FindByIdAddressCustomerHandler : GetByIdEntityHandler<AccountsAddressEntity> {
            public FindByIdAddressCustomerHandler(IEntityService<AccountsAddressEntity> service, IMapper mapper) : base(service, mapper) { }
        }
    }
}

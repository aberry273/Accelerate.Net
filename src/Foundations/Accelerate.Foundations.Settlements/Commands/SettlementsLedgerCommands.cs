using Accelerate.Foundations.Settlements.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Mediator.Commands;
using Accelerate.Foundations.Mediator.Queries;
using AutoMapper;
using MassTransit.Futures.Contracts;
using MediatR;

namespace Accelerate.Foundations.Settlements.Commands
{
    public class SettlementsBankAccountCommands
    {
        public class CreateBankAccountCustomerCommand : CreateEntityCommand<SettlementsLedgerEntity>;
        public class UpdateBankAccountCustomerCommand : UpdateEntityCommand<SettlementsLedgerEntity>;
        public class DeleteBankAccountCustomerCommand : DeleteEntityCommand<SettlementsLedgerEntity>;
        public class CreateBankAccountCustomerHandler : CreateEntityHandler<SettlementsLedgerEntity> {
            public CreateBankAccountCustomerHandler(BaseContext<SettlementsLedgerEntity> service, IMapper mapper, IMediator mediator) : base(service, mapper, mediator) { }
        }
        public class UpdateBankAccountCustomerHandler : UpdateEntityHandler<SettlementsLedgerEntity> {
            public UpdateBankAccountCustomerHandler(BaseContext<SettlementsLedgerEntity> service, IMapper mapper, IMediator mediator) : base(service, mapper, mediator) { }
        }
        public class DeleteBankAccountCustomerHandler : DeleteEntityHandler<SettlementsLedgerEntity> {
            public DeleteBankAccountCustomerHandler(BaseContext<SettlementsLedgerEntity> service, IMapper mapper, IMediator mediator) : base(service, mapper, mediator) { }
        }
        public class FindEntityBankAccountCustomerHandler : FindEntityHandler<SettlementsLedgerEntity> {
            public FindEntityBankAccountCustomerHandler(IEntityService<SettlementsLedgerEntity> service, IMapper mapper) : base(service, mapper) { }
        }
        public class FindByIdBankAccountCustomerHandler : GetByIdEntityHandler<SettlementsLedgerEntity> {
            public FindByIdBankAccountCustomerHandler(IEntityService<SettlementsLedgerEntity> service, IMapper mapper) : base(service, mapper) { }
        }
    }
}

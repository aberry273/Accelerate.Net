using Accelerate.Foundations.Funding.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Mediator.Commands;
using Accelerate.Foundations.Mediator.Queries;
using AutoMapper;
using MassTransit.Futures.Contracts;
using MediatR;

namespace Accelerate.Foundations.Funding.Commands
{
    public class FundingBankAccountCommands
    {
        public class CreateBankAccountCustomerCommand : CreateEntityCommand<FundingBankAccountEntity>;
        public class UpdateBankAccountCustomerCommand : UpdateEntityCommand<FundingBankAccountEntity>;
        public class DeleteBankAccountCustomerCommand : DeleteEntityCommand<FundingBankAccountEntity>;
        public class CreateBankAccountCustomerHandler : CreateEntityHandler<FundingBankAccountEntity> {
            public CreateBankAccountCustomerHandler(BaseContext<FundingBankAccountEntity> service, IMapper mapper, IMediator mediator) : base(service, mapper, mediator) { }
        }
        public class UpdateBankAccountCustomerHandler : UpdateEntityHandler<FundingBankAccountEntity> {
            public UpdateBankAccountCustomerHandler(BaseContext<FundingBankAccountEntity> service, IMapper mapper, IMediator mediator) : base(service, mapper, mediator) { }
        }
        public class DeleteBankAccountCustomerHandler : DeleteEntityHandler<FundingBankAccountEntity> {
            public DeleteBankAccountCustomerHandler(BaseContext<FundingBankAccountEntity> service, IMapper mapper, IMediator mediator) : base(service, mapper, mediator) { }
        }
        public class FindEntityBankAccountCustomerHandler : FindEntityHandler<FundingBankAccountEntity> {
            public FindEntityBankAccountCustomerHandler(IEntityService<FundingBankAccountEntity> service, IMapper mapper) : base(service, mapper) { }
        }
        public class FindByIdBankAccountCustomerHandler : GetByIdEntityHandler<FundingBankAccountEntity> {
            public FindByIdBankAccountCustomerHandler(IEntityService<FundingBankAccountEntity> service, IMapper mapper) : base(service, mapper) { }
        }
    }
}

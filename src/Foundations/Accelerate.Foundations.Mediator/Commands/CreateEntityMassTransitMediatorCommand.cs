using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Mediator.Models;
using AutoMapper;
using MassTransit.Mediator;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Mediator.Commands
{
    /*
    public class CreateEntityCommand<T> : IRequest<BaseResponse<bool>>
    {
        public required T Entity { get; set; }
    }*/
    public class CreateEntityMassTransitMediatorCommand<T> : MediatorRequestHandler<BaseResponse<T>> where T : BaseEntity
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public required T Entity { get; set; }
        //private readonly IUnitOfWork _unitOfWork;
        private readonly BaseContext<T> _context;
        //private readonly IEntityService<T> _service;
        private readonly IMapper _mapper;

        public CreateEntityMassTransitMediatorCommand(BaseContext<T> context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        // Update to unit of work

        public async Task<BaseResponse<bool>> Handle(CreateEntityCommand<T> command, CancellationToken cancellationToken)
        {

            var response = new BaseResponse<bool>();
            try
            {
                var entity = _mapper.Map<T>(command);
                var addResult = await _context.AddAsync(entity);
                var result = await _context.SaveChangesAsync();
                response.Data = result > 0;
                if (response.Data)
                {
                    response.Success = true;
                    response.Message = "Create successful";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Success = false;
            }
            return response;
        }

        protected override Task Handle(BaseResponse<T> request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

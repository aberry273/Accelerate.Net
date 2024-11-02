using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Mediator.Events;
using Accelerate.Foundations.Mediator.Models;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Mediator.Commands
{
    public class CreateEntityHandler<T> : IRequestHandler<CreateEntityCommand<T>, BaseResponse<bool>> where T: BaseEntity
    {
        //private readonly IUnitOfWork _unitOfWork;
        private readonly BaseContext<T> _context;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public CreateEntityHandler(BaseContext<T> context, IMapper mapper, IMediator mediator)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mapper));
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
                    await SendEvent(entity);
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Success = false;
            }
            return response;
        }

        private async Task SendEvent(T entity)
        {
            var name = typeof(T).Name;
            var ev = new EntityCreatedEvent<T>()
            {
                Entity = entity,
                EventName = $"{name}:created"
            };
            await _mediator.Publish(ev);
        }
    }
}

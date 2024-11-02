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
using static MassTransit.ValidationResultExtensions;

namespace Accelerate.Foundations.Mediator.Commands
{
    public class UpdateEntityHandler<T> : IRequestHandler<UpdateEntityCommand<T>, BaseResponse<bool>> where T : BaseEntity
    {
        //private readonly IUnitOfWork _unitOfWork;
        private readonly BaseContext<T> _context;
        //private readonly IEntityService<T> _service;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public UpdateEntityHandler(BaseContext<T> context, IMapper mapper, IMediator mediator)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<BaseResponse<bool>> Handle(UpdateEntityCommand<T> command, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>();
            try
            {
                var entity = _mapper.Map<T>(command);
                var addResult = _context.Update(entity);
                var result = await _context.SaveChangesAsync() > 0;
                if (result)
                {
                    response.Success = true;
                    response.Message = "Update successful";
                    await SendEvent(entity);
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
        private async Task SendEvent(T entity)
        {
            var name = typeof(T).Name;
            var ev = new EntityCreatedEvent<T>()
            {
                Entity = entity,
                EventName = $"{name}:updated"
            };
            await _mediator.Publish(ev);
        }
    }
}

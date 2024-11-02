using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Mediator.Events;
using Accelerate.Foundations.Mediator.Models;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Mediator.Commands
{
    public class DeleteEntityHandler<T> : IRequestHandler<DeleteEntityCommand<T>, BaseResponse<bool>> where T : BaseEntity
    {
        //private readonly IUnitOfWork _unitOfWork;
        private readonly BaseContext<T> _context;
        //private readonly IEntityService<T> _service;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public DeleteEntityHandler(BaseContext<T> context, IMapper mapper, IMediator mediator)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<BaseResponse<bool>> Handle(DeleteEntityCommand<T> command, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>();
            try
            {
                if (command.Id == null && command.Entity == null)
                {
                    response.Success = false;
                    response.Message = "No entity found";
                    return response;
                }
                var entity = command.Entity ?? _context.Entities.First(x => x.Id == command.Id.GetValueOrDefault());

                if (entity == null)
                {
                    response.Success = false;
                    response.Message = "No entity found";
                    return response;
                }

                var deleteResult = _context.Remove(entity);
                var result = await _context.SaveChangesAsync() > 0;

                if (result)
                {
                    response.Success = true;
                    response.Message = "Delete successful";
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
                EventName = $"{name}:deleted"
            };
            await _mediator.Publish(ev);
        }
    }
}

using Accelerate.Foundations.Database.Services;
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
    public class DeleteEntityHandler<T> : IRequestHandler<DeleteEntityCommand<T>, BaseResponse<bool>>
    {
        //private readonly IUnitOfWork _unitOfWork;
        //private readonly BaseContext<T> _context;
        private readonly IEntityService<T> _service;
        private readonly IMapper _mapper;

        public DeleteEntityHandler(IEntityService<T> service, IMapper mapper)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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
                }
                var entity = command.Entity ?? _service.Get(command.Id.GetValueOrDefault());

                response.Data = await _service.Delete(entity) > 0;

                if (response.Data)
                {
                    response.Success = true;
                    response.Message = "Delete successful";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
    }
}

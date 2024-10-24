using Accelerate.Foundations.Database.Models;
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
    public class CreateEntityHandler<T> : IRequestHandler<CreateEntityCommand<T>, BaseResponse<bool>> where T: IBaseEntity
    {
        //private readonly IUnitOfWork _unitOfWork;
        //private readonly BaseContext<T> _context;
        private readonly IEntityService<T> _service;
        private readonly IMapper _mapper;

        public CreateEntityHandler(IEntityService<T> service, IMapper mapper)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<BaseResponse<bool>> Handle(CreateEntityCommand<T> command, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>();
            try
            {
                var entity = _mapper.Map<T>(command);
                response.Data = await _service.Create(entity) > 0;
                if (response.Data)
                {
                    response.Success = true;
                    response.Message = "Create successful";
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

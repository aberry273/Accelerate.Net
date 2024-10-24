using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Mediator.Commands;
using Accelerate.Foundations.Mediator.Models;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Mediator.Queries
{
    public class GetByIdEntityHandler<T> : IRequestHandler<GetIdEntityQuery<T>, BaseResponse<T>>
    {

        private readonly IEntityService<T> _service;
        private readonly IMapper _mapper;
        public GetByIdEntityHandler(IEntityService<T> service, IMapper mapper)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<BaseResponse<T>> Handle(GetIdEntityQuery<T> request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<T>();
            try
            {
                var entity = _service.Get(request.Id);
                if (entity != null)
                {
                    response.Data = _mapper.Map<T>(entity);
                    response.Success = true;
                    response.Message = "Create successful!";
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
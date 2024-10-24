using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Mediator.Commands;
using Accelerate.Foundations.Mediator.Models;
using AutoMapper;
using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Mediator.Queries
{
    public class FindEntityHandler<T> : IRequestHandler<FindEntityQuery<T>, BaseResponsePagination<IEnumerable<T>>>
    {

        private readonly IEntityService<T> _service;
        private readonly IMapper _mapper;
        public FindEntityHandler(IEntityService<T> service, IMapper mapper)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<BaseResponsePagination<IEnumerable<T>>> Handle(FindEntityQuery<T> request, CancellationToken cancellationToken)
        {
            var response = new BaseResponsePagination<IEnumerable<T>>();
            try
            {
                var count = _service.Count(request.Expression);
                var entities = _service.Find(request.Expression, request.Page * request.PageSize, request.PageSize);

                if (entities is not null)
                {
                    response.Page = request.Page;
                    response.TotalPages = (int)Math.Ceiling(count / (double)request.PageSize);
                    response.Total = count;
                    response.Data = _mapper.Map<IEnumerable<T>>(entities);
                    response.Success = true;
                    response.Message = "Query succeed!";
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
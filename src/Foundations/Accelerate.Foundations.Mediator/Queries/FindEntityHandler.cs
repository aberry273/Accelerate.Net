using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Mediator.Commands;
using Accelerate.Foundations.Mediator.Models;
using AutoMapper;
using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Accelerate.Foundations.Database.Constants.Exceptions;

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
        private Expression<Func<T, bool>> CreateExpression(T query)
        {
            ParameterExpression argParam = Expression.Parameter(typeof(T), "s");
            Expression nameProperty = Expression.Property(argParam, "Name");
            Expression namespaceProperty = Expression.Property(argParam, "Namespace");

            var val1 = Expression.Constant("Modules");
            var val2 = Expression.Constant("Namespace");

            Expression e1 = Expression.Equal(nameProperty, val1);
            Expression e2 = Expression.Equal(namespaceProperty, val2);
            var andExp = Expression.AndAlso(e1, e2);

            var lambda = Expression.Lambda<Func<T, bool>>(andExp, argParam);
            return lambda;
        }
        public async Task<BaseResponsePagination<IEnumerable<T>>> Handle(FindEntityQuery<T> request, CancellationToken cancellationToken)
        {
            var response = new BaseResponsePagination<IEnumerable<T>>();
            try
            {
                var expression = CreateExpression(request.Query);
                var count = _service.Count(expression);
                var entities = _service.Find(expression, request.Page * request.PageSize, request.PageSize);

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
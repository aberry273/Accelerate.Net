using Accelerate.Foundations.Mediator.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Mediator.Queries
{
    public class FindEntityQuery<T> : IRequest<BaseResponsePagination<IEnumerable<T>>>
    {
        public Expression<Func<T, bool>>? Expression { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

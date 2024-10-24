using Accelerate.Foundations.Mediator.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Mediator.Queries
{
    public class GetIdEntityQuery<T> : IRequest<BaseResponsePagination<T>>
    {
        public Guid Id { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

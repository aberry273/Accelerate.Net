using Accelerate.Foundations.Mediator.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Mediator.Commands
{
    public class DeleteEntityCommand<T> : IRequest<BaseResponse<bool>>
    {
        public Guid? Id { get; set; }
        public T Entity { get; set; }
    }
}

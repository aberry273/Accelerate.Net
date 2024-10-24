using Accelerate.Foundations.Mediator.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Mediator.Commands
{
    public class UpdateEntityCommand<T> : IRequest<BaseResponse<bool>>
    {
        public required T Entity { get; set; }
    }
}

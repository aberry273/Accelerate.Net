using Accelerate.Foundations.Mediator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Mediator.Exceptions
{
    public class ValidationExceptionCustom : Exception
    {
        public IEnumerable<BaseError> Errors { get; }

        public ValidationExceptionCustom()
            : base("One or more validation failures have occured.")
        {
            Errors = new List<BaseError>();
        }

        public ValidationExceptionCustom(IEnumerable<BaseError> errors)
            : this()
        {
            Errors = errors;
        }
    }
}

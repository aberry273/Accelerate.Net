using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Pipelines
{
    public interface IPipelineArgs<T>
    {
        /// <summary>
        /// Set whatever values are wanted for further pipeline processes
        /// </summary>
        public dynamic Params { get; set; } 
        public T Value { get; set; }
    }
}

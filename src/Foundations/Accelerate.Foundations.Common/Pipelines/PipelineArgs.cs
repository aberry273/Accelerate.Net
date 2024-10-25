using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Pipelines
{
    public class PipelineArgs<T> : IPipelineArgs<T>
    {
        public required T Value { get; set; }
        public dynamic Params { get; set; } = new Dictionary<string, dynamic>();
    }
}

using Accelerate.Foundations.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Pipelines
{
    public interface IPipelineProcessor<T>
    {
        void Run(T obj);
        Task RunAsync(T obj);
    }
}

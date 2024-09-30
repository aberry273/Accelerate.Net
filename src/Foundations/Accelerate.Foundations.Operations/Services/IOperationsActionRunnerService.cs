using Accelerate.Foundations.Common.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Operations.Services
{
    public interface IOperationsActionRunnerService
    {
        Task<OperationResponse<object>> Run(Guid? jobId, Guid actionId, string type, string data, string settings);
    }
}

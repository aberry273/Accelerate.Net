using Accelerate.Foundations.Operations.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Operations.Services
{
    public interface IOperationsJobSchedulerService
    {
        OperationsJobEntity[] GetJobEntities();
        OperationsActionEntity GetJobAction(Guid? actionId);
        void CreateJobActivity(Guid? jobId, Guid actionId, string data, string result, bool success);
    }
}

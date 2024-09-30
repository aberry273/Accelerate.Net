using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Accelerate.Foundations.Integrations.Quartz;
using System.Threading.Tasks;
using Accelerate.Foundations.Operations.Services;

namespace Accelerate.Services.JobScheduler.Jobs
{
    public class ActionJob : IJob
    {
        private static IOperationsActionRunnerService _operationActionRunnerService => new OperationsActionRunnerService();
        public async Task Execute(IJobExecutionContext context)
        {
            //Get Action Entity
            await this.Run(context);
        }

        private async Task Run(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            var jobName = dataMap.GetString(Constants.Settings.Keys.ActionName);
            Console.WriteLine($"Starting '{jobName}'");
            await _operationActionRunnerService.Run(
                Guid.Parse(dataMap.GetString(Constants.Settings.Keys.JobId)),
                Guid.Parse(dataMap.GetString(Constants.Settings.Keys.ActionId)),
                dataMap.GetString(Constants.Settings.Keys.Type),
                dataMap.GetString(Constants.Settings.Keys.Data),
                dataMap.GetString(Constants.Settings.Keys.Settings));

            Console.WriteLine($"Finished '{jobName}'");
        }
    }
}

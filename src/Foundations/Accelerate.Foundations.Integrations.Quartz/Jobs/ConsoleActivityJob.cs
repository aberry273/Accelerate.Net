using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Integrations.Quartz.Jobs
{
    public class ConsoleActivityJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Console.Out.WriteLineAsync("Job schedule running");
        }
    }
}

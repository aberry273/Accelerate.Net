using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Integrations.Quartz.Services
{
    public interface IQuartzService
    {
        Task<IScheduler> GetScheduler();
        Task ShutdownScheduler();

        Task InitDefaultSchedules(string groupName);
        ITrigger CreateTrigger(string triggerName, string group, int seconds);
        ITrigger CreateTrigger(string triggerName, string jobName, string group, string schedule);
        IJobDetail CreateJob<T>(string jobName, string group) where T : IJob;
        IJobDetail CreateJob(string jobName, string group);
        Task ScheduleActionJob<T>(string jobId, string actionId, string actionName, string actionType, string actionData, string actionSettings, string group, string schedule) where T : IJob;
    }
}

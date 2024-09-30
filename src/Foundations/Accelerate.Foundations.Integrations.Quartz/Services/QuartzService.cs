using Accelerate.Foundations.Integrations.Quartz.Jobs;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Integrations.Quartz.Services
{

    public class QuartzService : IQuartzService
    {
        private StdSchedulerFactory _factory;
        private IScheduler _scheduler;
        private const string _activityJobName = "activityJob";
        private const string _activityTriggerName = "activityTrigger";
        public QuartzService()
        {
            _factory = new StdSchedulerFactory();
        }
        public async Task InitScheduler()
        {
            // Grab the Scheduler instance from the Factory
            _scheduler = await _factory.GetScheduler();
            // and start it off
            await _scheduler.Start();
        }
        public async Task<IScheduler> GetScheduler()
        {
            if (_scheduler == null) await this.InitScheduler();
            return _scheduler;
        }
        public async Task ShutdownScheduler()
        {
            await _scheduler.Shutdown();
        }
        public async Task InitDefaultSchedules(string groupName)
        {
            // Create a job to regularly output to console
            var activityJob = this.CreateJob<ConsoleActivityJob>(_activityJobName, groupName);
            var activityTrigger = this.CreateTrigger(_activityTriggerName, groupName, 30);

            // Tell quartz to schedule the job using our trigger
            await _scheduler.ScheduleJob(activityJob, activityTrigger);
        }
        public async Task ScheduleActionJob<T>(string jobId, string actionId, string actionName, string actionType, string actionData, string actionSettings, string group, string schedule) where T : IJob
        {
            var fixedTriggerName = $"{jobId}/{actionId}/{actionName}.trigger";
            var fixedjobName = $"{jobId}/{actionId}/{actionName}.job";

            var trigger = this.CreateTrigger(fixedTriggerName, fixedjobName, group, schedule);

            var job = JobBuilder.Create<T>()
               .WithIdentity(fixedjobName, group)
               .UsingJobData(Constants.Settings.Keys.ActionName, actionName)
               .UsingJobData(Constants.Settings.Keys.JobId, jobId)
               .UsingJobData(Constants.Settings.Keys.ActionId, actionId)
               .UsingJobData(Constants.Settings.Keys.Type, actionType)
               .UsingJobData(Constants.Settings.Keys.Data, actionData)
               .UsingJobData(Constants.Settings.Keys.Settings, actionSettings)
               .Build();

            await _scheduler.ScheduleJob(job, trigger);
        }
        public ITrigger CreateTrigger(string triggerName, string jobName, string group, string schedule)
        {
            // Trigger the job to run now, and then repeat every 10 seconds
            return TriggerBuilder.Create()
                .WithIdentity(triggerName, group)
                .WithCronSchedule(schedule)
                .ForJob(jobName, group)
                .Build();
        }
        public ITrigger CreateTrigger(string triggerName, string group, int seconds)
        {
            // Trigger the job to run now, and then repeat every 10 seconds
            return TriggerBuilder.Create()
                .WithIdentity(triggerName, group)
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(seconds)
                    .RepeatForever())
                .Build();
        }
        public IJobDetail CreateJob(string jobName, string group)
        {
            // Trigger the job to run now, and then repeat every 10 seconds
            return JobBuilder.Create<BaseJob>()
                .WithIdentity(jobName, group)
                .Build();
        }
        public IJobDetail CreateJob<T>(string jobName, string group) where T : IJob
        {
            // Trigger the job to run now, and then repeat every 10 seconds
            return JobBuilder.Create<T>()
                .WithIdentity(jobName, group)
                .Build();
        }
    }
}

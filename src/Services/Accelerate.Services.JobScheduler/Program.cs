
using Accelerate.Services.JobScheduler.Jobs;
using Accelerate.Services.JobScheduler;
using Microsoft.Extensions.Configuration;
using Accelerate.Foundations.Integrations.Quartz.Services;
using Quartz.Logging;
using Accelerate.Foundations.Operations.Services;

string _apRegionGroup = "apac";

LogProvider.SetCurrentLogProvider(new FileLogProvider());
//GetConfigFile
// Build a config object, using env vars and JSON providers.

/*
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

Accelerator.Foundations.Operations.Startup.ConfigureSingletonServices(config);
*/
// Get values from the config given their key and their target type.


var _quartzService = new QuartzService();
// Initialize the scheduler (create/start)
// Grab the active (started) Scheduler instance from the Factory
await _quartzService.InitScheduler();
// Create&Schedule a default activity logging job/trigger
await _quartzService.InitDefaultSchedules(_apRegionGroup);

var _schedulerService = new OperationsJobSchedulerService();

// Read all jobs from schedule
//using (var context = new JobsSchedulerDbContext(config))
{
    //TODO: FIX having two jobs with 1 action causes the new (bulkemail) action to run instead of the job

    // Read all jobs with a schedule and action
    var jobs = _schedulerService.GetJobEntities();
    Console.WriteLine($"Loaded {jobs.Count()} jobs");
    for (var i = 0; i < jobs.Count(); i++)
    {
        var job = jobs[i];
        var action = _schedulerService.GetJobAction(job.ActionId);

        if (action == null)
        {
            Console.WriteLine($"Scheduled '{job.Name} ({job.Id})' assigned action not found {job.ActionId}'");
            continue;
        }
        await _quartzService.ScheduleActionJob<ActionJob>(
            job.Id.ToString(),
            job.ActionId.ToString(),
            $"{job.Name}",
            action.Action,
            action.Data,
            action.Settings,
            _apRegionGroup,
            job.Schedule);
        Console.WriteLine($"Scheduled '{job.Name}' on schedule '{job.Schedule}'");
    }
    Console.WriteLine($"Scheduled {jobs.Count()} jobs");
}
Console.WriteLine("Press any key to close the application");
Console.ReadKey();

// and last shut down the scheduler when you are ready to close your program
await _quartzService.ShutdownScheduler();
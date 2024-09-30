using Accelerate.Foundations.Operations.Data;
using Accelerate.Foundations.Operations.Models;
using Accelerate.Foundations.Operations.Models.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Operations.Services
{
    public class OperationsJobSchedulerService : IOperationsJobSchedulerService
    {
        private readonly IConfiguration _configuration;
        public OperationsJobSchedulerService()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                //.AddEnvironmentVariables()
                .Build();

            // Get values from the config given their key and their target type.
            //Settings settings = config.GetRequiredSection("Settings").Get<Settings>();
            //var connStr = config.GetConnectionString(Foundations.Common.Constants.Site.Settings.ConnectionStringName);
        }
        public OperationsJobEntity[] GetJobEntities()
        {
            using (var context = new OperationsJobSchedulerDbContext(_configuration))
            {
                return context.Jobs.Where(x =>
                x.ActionId != null
                && !string.IsNullOrEmpty(x.Schedule)
                && x.State == OperationsJobState.Published).ToArray();
            }
        }
        public OperationsActionEntity GetJobAction(Guid? actionId)
        {
            if (actionId == null) return null;
            using (var context = new OperationsJobSchedulerDbContext( _configuration))
            {
                return context.Actions.FirstOrDefault(x => x.Id == actionId);
            }
        }

        public void CreateJobActivity(Guid? jobId, Guid actionId, string data, string result, bool success)
        {
            using (var context = new OperationsJobSchedulerDbContext(_configuration))
            {
                var actionActivity = new OperationsActivityEntity()
                {
                    OperationsJobId = jobId,
                    OperationsActionId = actionId,
                    Data = data,
                    Result = result,
                    Success = success
                };
                context.Activities.Add(actionActivity);
                context.SaveChanges();
            }
        }
    }
}

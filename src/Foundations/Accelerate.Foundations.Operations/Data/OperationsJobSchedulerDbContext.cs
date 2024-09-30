using Accelerate.Foundations.Operations.Models.Data;
using Accelerate.Foundations.Operations.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Accelerate.Foundations.Database.Constants.Exceptions;

namespace Accelerate.Foundations.Operations.Data
{
    public class OperationsJobSchedulerDbContext : DbContext
    {
        private IConfiguration _config { get; set; }
        //private IServiceCollection _services { get; set; }
        public OperationsJobSchedulerDbContext(IConfiguration config)
        {
            _config = config;
            //_services = services;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var environment = _config[Constants.Config.Environment];
            var isProduction = environment == "Production";
            var connString = isProduction ? _config[Constants.Config.DatabaseKey] : _config.GetConnectionString(Constants.Config.LocalDatabaseKey);
            optionsBuilder.UseSqlServer(connString);
        }
        public DbSet<OperationsActivityEntity> Activities { get; set; }
        public DbSet<OperationsActionEntity> Actions { get; set; }
        public DbSet<OperationsJobEntity> Jobs { get; set; }
    }
}

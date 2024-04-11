using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Integrations.AzureStorage
{
    public struct Constants
    {
        public struct Settings
        {
            public const string AppContainerName = "parrotmvp";
    }
        public struct Config
        {
            public const string SectionName = "AzureStorageConfiguration";
            public const string AccessKey = "AzureStorageAccessKey";
            public const string ConnectionString = "AzureStorageConnectionString";
            public const string AccountName = "parrotmvp";
        }
    }
}
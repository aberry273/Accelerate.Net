
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Accounts
{
    public struct Constants
    {
        public struct Defaults
        {
            
        }
        public struct Roles
        {
            public const string UserAccountIndividualName = "Individual";
            public const string UserAccountIndividualDescription = "Represents a customer that is a consumer in Superstable";
            public const string UserAccountBusinessName = "Business";
            public const string UserAccountBusinessDescription = "Represents a customer that is a Business in Superstable";
        }
        public struct Config
        {
            public const string ConfigName = "ContentConfiguration";
            public const string PostIndexName = "PostIndexName";
            public const string ChannelIndexName = "ChannelIndexName";
            public const string ActionsIndexName = "ActionsIndexName";
            public const string ActivitiesIndexName = "ActivitiesIndexName";

            public const string LocalDatabaseKey = "LocalAccountsContext";
            public const string DatabaseKey = "AccountsContext";
        }
    }
}
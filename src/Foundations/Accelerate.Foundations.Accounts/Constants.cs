
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
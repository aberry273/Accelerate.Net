using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Account
{
    public struct Constants
    {
        public struct Fields
        {
            public const string Username = "username";
        }
        public struct Config
        {
            public const string ConfigName = "AccountConfiguration";
            public const string UserIndexName = "UserIndexName";

            public const string LocalDatabaseKey = "LocalAccountContext";
            public const string DatabaseKey = "AccountContext";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Funding
{
    public struct Constants
    {
        public struct Defaults
        {
            
        }
        public struct Config
        {
            public const string ConfigName = "FundingConfiguration";
            public const string LocalDatabaseKey = "LocalFundingContext";
            public const string DatabaseKey = "FundingContext";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Rates
{
    public struct Constants
    {
        public struct Defaults
        {
            
        }
        public struct Config
        {
            public const string ConfigName = "RatesConfiguration";
            public const string LocalDatabaseKey = "LocalRatesContext";
            public const string DatabaseKey = "RatesContext";
        }
    }
}
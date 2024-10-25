
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Kyc
{
    public struct Constants
    {
        public struct Defaults
        {
            
        }
        public struct Config
        {
            public const string ConfigName = "KycConfiguration";
            public const string LocalDatabaseKey = "LocalKycContext";
            public const string DatabaseKey = "KycContext";
        }
    }
}
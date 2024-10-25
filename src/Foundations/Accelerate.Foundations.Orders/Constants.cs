
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Orders
{
    public struct Constants
    {
        public struct Defaults
        {
            
        }
        public struct Config
        {
            public const string ConfigName = "OrdersConfiguration";
            public const string LocalDatabaseKey = "LocalOrdersContext";
            public const string DatabaseKey = "OrdersContext";
        }
    }
}
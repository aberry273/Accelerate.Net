
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Transactions
{
    public struct Constants
    {
        public struct Defaults
        {
            
        }
        public struct Config
        {
            public const string ConfigName = "TransactionsConfiguration";
            public const string LocalDatabaseKey = "LocalTransactionsContext";
            public const string DatabaseKey = "TransactionsContext";
        }
    }
}
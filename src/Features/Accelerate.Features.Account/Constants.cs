using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Features.Account
{
    public struct Constants
    {
        public struct Filters
        {
            public struct Media
            {
                public const string Type = "Type";
                public const string Tags = "Tags";
            }
            public struct Posts
            {
                public const string Actions = "Actions";
                public const string Threads = "Threads";
                public const string Quotes = "Quotes";
                public const string Tags = "Tags";
                public const string Status = "Status";
                public const string Content = "Content";
                public const string Sort = "Sort";
            }
        }
    }
}
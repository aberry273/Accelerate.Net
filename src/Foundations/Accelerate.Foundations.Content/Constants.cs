using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Content
{
    public struct Constants
    {
        public struct Fields
        {
            public const string UserId = "userId";
            public const string TargetThread = "targetThread";
            public const string ContentPostId = "contentPostId";
        }
        public struct Search
        {
            public const int MaxQueryable = 100;
            public const int DefaultPerPage = 10;
        }
        public struct Settings
        {
            public const string ConnectionStringName = "ContentContext";
        }
    }
}
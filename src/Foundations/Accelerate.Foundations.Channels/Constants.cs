using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Channels
{
    public struct Constants
    {
        public struct Fields
        {
            public const string UserId = "userId";
            public const string SelfReply = "selfReply";
            public const string threadId = "threadId";
            public const string TargetThread = "targetThread";
            public const string ContentPostId = "contentPostId";


            public const string Reviews = "Reviews";
            public const string Threads = "Threads";
            public const string Tags = "Tags";
            public const string Status = "Status";
            public const string Content = "Content";
            public const string Sort = "Sort";
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
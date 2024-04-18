using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Features.Content
{
    public struct Constants
    {
        public struct Filters
        {
            public const string Reviews = "Reviews";
            public const string Threads = "Threads";
            public const string Quotes = "Quotes";
            public const string Tags = "Tags";
            public const string Status = "Status";
            public const string Content = "Content";
            public const string Sort = "Sort";
        }
        public struct Settings
        {
            public const string ContentPostsHubName = "ContentPosts";
            public const string ContentPostReviewsHubName = "ContentPostReviews";
            public const string ContentChannelsHubName = "ContentChannels";
            public const string ContentPostQuotesHubName = "ContentPostQuotes";

        }
    }
}
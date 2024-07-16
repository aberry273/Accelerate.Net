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
            public const string Actions = "Actions";
            public const string Threads = "Threads";
            public const string Quotes = "Quotes";
            public const string Category = "Category";
            public const string Labels = "Labels";
            public const string Tags = "Tags";
            public const string Votes = "Votes";
            public const string Status = "Status";
            public const string Content = "Content";
            public const string Sort = "Sort";
            public const string SortOrder = "SortOrder";
        }
        public struct Settings
        {
            public const string ContentPostsHubName = "ContentPosts";
            public const string ContentPostActionsHubName = "ContentPostActions";
            public const string ContentPostActionsSummaryHubName = "ContentPostActionsSummary";
            public const string ContentChannelsHubName = "ContentChannels";
            public const string ContentPostActivitiesHubName = "ContentPostActivities";
            public const string ContentPostQuotesHubName = "ContentPostQuotes";
            public const string ContentPostSettingsHubName = "ContentPostSettingsSummary";

        }
    }
}
﻿using Accelerate.Foundations.Content.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Content
{
    public struct Constants
    {
        public struct Defaults
        {
            public struct Posts
            {
                public const ContentPostEntityStatus Status = ContentPostEntityStatus.Public;
            }
            
        }
        public struct Config
        {
            public const string ConfigName = "ContentConfiguration";
            public const string PostIndexName = "PostIndexName";
            public const string ChannelIndexName = "ChannelIndexName";
            public const string ActionsIndexName = "ActionsIndexName";
            public const string ActivitiesIndexName = "ActivitiesIndexName";

            public const string LocalDatabaseKey = "LocalContentContext";
            public const string DatabaseKey = "ContentContext";
        }
        public struct Fields
        {
            public const string Description = "description";
            public const string UserId = "userId";
            public const string PostType = "postType";
            public const string ThreadId = "threadId";
            public const string ShortThreadId = "shortThreadId";
            public const string ParentId = "parentId";
            public const string ParentIds = "parentIds";
            public const string threadId = "threadId";
            public const string ChannelId = "channelId";
            public const string CreatedOn = "createdOn";
            public const string UpdatedOn = "updatedOn";
            public const string Replies = "replies";
            public const string Quotes = "quotes";
            public const string TotalVotes = "totalVotes";
            public const string ContentPostId = "contentPostId";
            public const string QuoteIds = "quoteIds";
            public const string Id = "id";

            public const string Category = "category";
            public const string Tags = "tags";
            public const string Labels = "taxonomy.labels";
            public const string ParentVote = "parentVote";
            public const string Status = "status";
            public const string Content = "content";
        }
    }
}
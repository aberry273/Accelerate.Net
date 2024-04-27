﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Content
{
    public struct Constants
    {
        public struct Config
        {
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
            public const string TargetThread = "targetThread";
            public const string TargetChannel = "targetChannel";
            public const string ContentPostId = "contentPostId";
            public const string QuoteIds = "quoteIds";
            public const string Id = "id";

            public const string Category = "Category";
            public const string Tags = "tags";
            public const string Status = "status";
            public const string Content = "content";
        }
        public struct Search
        {
            public const int MaxQueryable = 100;
            public const int DefaultPerPage = 10;
        }
    }
}
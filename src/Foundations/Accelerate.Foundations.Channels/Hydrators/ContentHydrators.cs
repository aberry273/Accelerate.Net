using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Elastic.Clients.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Elastic.Clients.Elasticsearch.JoinField;

namespace Accelerate.Foundations.Channels.Hydrators
{
    public static class ContentHydrators
    {
        public static void HydrateDocument(this ContentPostEntity entity, ContentPostDocument document, string username)
        {
            document.Status = entity.Status;
            document.Content = entity.Content;
            document.UserId = entity.UserId;
            document.CreatedOn = entity.CreatedOn;
            document.UpdatedOn = entity.UpdatedOn;
            document.ThreadId = entity.ThreadId;
            document.TargetThread = entity.TargetThread;
            document.ParentId = entity.ParentId;
            document.TargetChannel = entity.TargetChannel;
            document.Tags = entity.TagItems;
            document.Category = entity.Category;
            document.Id = entity.Id;
            document.Username = username ?? "Anonymous";
        }
        public static void HydrateDocument(this ContentPostReviewEntity entity, ContentPostReviewDocument document)
        {
            document.UserId = entity.UserId;
            document.CreatedOn = entity.CreatedOn;
            document.UpdatedOn = entity.UpdatedOn;
            document.ContentPostId = entity.ContentPostId;
            document.Id = entity.Id;
            document.Agree = entity.Agree;
            document.Disagree = entity.Disagree;
            document.Like = entity.Like;
        }
    }
}

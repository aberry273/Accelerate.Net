using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.Content.Models.Data;
using Elastic.Clients.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Elastic.Clients.Elasticsearch.JoinField;

namespace Accelerate.Foundations.Common.Extensions
{
    public static class ContentHydrators
    {
        public static void HydrateDocument(this ContentPostEntity entity, ContentPostDocument document, string username)
        {
            document.Status = entity.Status;
            document.Content = entity.Content;
            document.UserId = entity.UserId;
            document.CreatedOn = entity.CreatedOn;
            document.TargetThread = entity.TargetThread;
            document.ParentId = entity.ParentId;
            document.TargetChannel = entity.TargetChannel;
            document.Tags = entity.TagItems;
            document.Category = entity.Category;
            document.Id = entity.Id;
            document.Username = username ?? "Anonymous";
        }
    }
}

using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;

namespace Accelerate.Foundations.Content.Hydrators
{
    public static class ContentHydrators
    {
        public static void Hydrate(this ContentPostEntity entity, ContentPostDocument document, string username)
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
        public static void Hydrate(this ContentPostReviewEntity entity, ContentPostReviewDocument document)
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
        public static void Hydrate(this ContentChannelEntity entity, ContentChannelDocument document)
        {
            document.UserId = entity.UserId;
            document.CreatedOn = entity.CreatedOn;
            document.UpdatedOn = entity.UpdatedOn;
            document.Name = entity.Name;
            document.Tags = entity.TagItems;
            document.Category = entity.Category;
            document.Description = entity.Description;
            document.Id = entity.Id;
        }
        public static void Hydrate(this ContentPostActivityEntity entity, ContentPostActivityDocument document)
        {
            document.UserId = entity.UserId;
            document.CreatedOn = entity.CreatedOn;
            document.UpdatedOn = entity.UpdatedOn;
            document.Action = entity.Action;
            document.Id = entity.Id;
        }
    }
}

using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Common.Extensions;

namespace Accelerate.Foundations.Content.Hydrators
{
    public static class ContentHydrators
    {
        public static void HydrateFrom(this ContentPostEntity entity, ContentPostEntity document)
        {
            document.Status = entity.Status;
            document.Content = entity.Content;
            document.UserId = entity.UserId;
            document.CreatedOn = entity.CreatedOn;
            document.UpdatedOn = entity.UpdatedOn;
            //document.Tags = entity.Tags;
            //document.Category = entity.Category;
            document.Id = entity.Id;
        }
        public static void Hydrate(this ContentPostEntity entity, ContentPostDocument document, ContentPostUserSubdocument profile)
        {
            document.Status = entity.Status;
            document.Content = entity.Content;
            document.UserId = entity.UserId;
            document.CreatedOn = entity.CreatedOn;
            document.UpdatedOn = entity.UpdatedOn;
            document.ThreadId = entity.ThreadId;
            document.ShortThreadId = Foundations.Common.Extensions.GuidExtensions.ShortenBase64(entity.ThreadId);
            document.PostType = entity.Type;
            /*
             * TODO: Move these into subdocument updates
            document.threadId = entity.threadId;
            document.ParentId = entity.ParentId;
            document.ParentIds = entity.ParentIdItems?.ToList();
            document.channelId = entity.channelId;

            
            document.Tags = entity.TagItems;
            document.Category = entity.Category;
            */
            document.Id = entity.Id;
            document.Profile = profile ?? 
                new ContentPostUserSubdocument()
                {
                    Username = "Anonymous"
                };
        }
        public static void Hydrate(this ContentPostLabelEntity entity, ContentPostLabelDocument document)
        {
            document.UserId = entity.UserId;
            document.CreatedOn = entity.CreatedOn;
            document.UpdatedOn = entity.UpdatedOn;
            document.ContentPostId = entity.ContentPostId;
            document.Label = entity.Label;
            document.Reason = entity.Reason;
            document.Id = entity.Id;
        }
        public static void Hydrate(this ContentPostMentionEntity entity, ContentPostMentionDocument document)
        {
            document.UserId = entity.UserId;
            document.CreatedOn = entity.CreatedOn;
            document.UpdatedOn = entity.UpdatedOn;
            document.ContentPostId = entity.ContentPostId;
            document.Id = entity.Id;
        }
        public static void Hydrate(this ContentPostActionsEntity entity, ContentPostActionsDocument document)
        {
            document.UserId = entity.UserId;
            document.CreatedOn = entity.CreatedOn;
            document.UpdatedOn = entity.UpdatedOn;
            document.ContentPostId = entity.ContentPostId;
            document.Id = entity.Id;
            document.Agree = entity.Agree;
            document.Disagree = entity.Disagree;
            document.Like = entity.Like;
            document.Reaction = entity.Reaction;
        }
        public static void Hydrate(this ContentPostActionsSummaryEntity entity, ContentPostActionsSummaryDocument document)
        {
            document.ContentPostId = entity.ContentPostId;
            document.Id = entity.Id;
            document.Agrees = entity.Agrees;
            document.Disagrees = entity.Disagrees;
            document.Quotes = entity.Quotes;
            document.Replies = entity.Replies;
        }

        public static void HydrateFrom(this ContentPostActionsSummaryDocument doc, ContentPostActionsSummaryDocument from)
        {
            doc.Agrees = from.Agrees;
            doc.Disagrees = from.Disagrees;
            doc.Likes = from.Likes;
            doc.Replies = from.Replies;
            doc.Quotes = from.Quotes;
        }

        public static void HydrateFrom(this ContentPostActionsEntity entity, ContentPostActionsEntity from)
        {
            entity.UpdatedOn = DateTime.Now;
            entity.Like = from.Like;
            entity.Agree = from.Agree;
            entity.Disagree = from.Disagree;
        }

        public static void Hydrate(this ContentPostLinkEntity entity, ContentPostLinkDocument document)
        {
            document.CreatedOn = entity.CreatedOn;
            document.UpdatedOn = entity.UpdatedOn;
            document.Title = entity.Title;
            document.Description = entity.Description;
            document.ShortUrl = entity.ShortUrl;
            document.Image = entity.Image;
            document.Url = entity.Url;
            document.Id = entity.Id;
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
            document.Message = entity.Message;
            document.Url = entity.Url;
            document.CreatedOn = entity.CreatedOn;
            document.UpdatedOn = entity.UpdatedOn;
            document.Action = entity.Action;
            document.Id = entity.Id;
        }
        public static void Hydrate(this ContentPostQuoteEntity entity, ContentPostQuoteDocument document)
        {
            document.UserId = entity.UserId;
            document.CreatedOn = entity.CreatedOn;
            document.UpdatedOn = entity.UpdatedOn;
            document.ContentPostId = entity.ContentPostId;
            document.QuotedContentPostId = entity.QuotedContentPostId;
            document.Id = entity.Id;
        }
    }
}

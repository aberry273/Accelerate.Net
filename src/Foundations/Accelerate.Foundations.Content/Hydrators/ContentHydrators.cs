using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Content.Models.View;

namespace Accelerate.Foundations.Content.Hydrators
{
    public static class ContentHydrators
    {
        public static void HydrateFrom(this ContentPostEntity entity, ContentPostEntity document)
        {
            document.Status = entity.Status;
            document.Text = entity.Text;
            document.UserId = entity.UserId;
            document.CreatedOn = entity.CreatedOn;
            document.UpdatedOn = entity.UpdatedOn;
            //document.Tags = entity.Tags;
            //document.Category = entity.Category;
            document.Id = entity.Id;
        }
        public static void HydrateFrom(this ContentPostViewDocument viewModel,  ContentPostDocument doc)
        {
            viewModel.Id = doc.Id;
            viewModel.UpdatedOn = doc.UpdatedOn;
            viewModel.CreatedOn = doc.CreatedOn;
            viewModel.UserId = doc.UserId;
            viewModel.ExternalId = doc.ExternalId;
            viewModel.Status = doc.Status;
            viewModel.Content = doc.Content;
            viewModel.Related = doc.Related;
            viewModel.Quotes = doc.Quotes;
            viewModel.Metrics = doc.Metrics;
            viewModel.Taxonomy = doc.Taxonomy;
            viewModel.Link = doc.Link;
        }
        private static string GetPostName(ContentPostDocument document)
        {
            var dateStr = Foundations.Common.Extensions.DateExtensions.ToDateShort(document.UpdatedOn);
            var isReply = document.Related.ParentId != null;
            var postPrefix = !isReply ? "Posted" : $"Replied to {document.Profile.Username}";
            var text = string.Empty;
            if (!string.IsNullOrEmpty(document.Content.Text))
            {
                int textMax = document.Content.Text.Length <= 32 ? document.Content.Text.Length : 32;
                text = $"{document.Content.Text.Substring(0, textMax)} - ";
            }
            return $"{text} {postPrefix} on {dateStr}";
        }
        private static string GetPostDescription(ContentPostDocument document)
        {
            if (document.Content.Text == null && document.Content.Formats == null) return string.Empty;
            if (!string.IsNullOrEmpty(document.Content.Text))
            {
                int textMax = document.Content.Text.Length <= 128 ? document.Content.Text.Length : 128;
                return document.Content.Text.Substring(0, textMax);
            }
            if (document.Content.Formats.FirstOrDefault() == null) return string.Empty;
            var text = document.Content.Formats.FirstOrDefault()?.data?.text ?? string.Empty;
            int formatMax = text.Length <= 128 ? text.Length : 128;
            return text.Substring(0, formatMax);
        }
        public static void Hydrate(this ContentPostEntity entity, ContentPostDocument document)
        {
            document.Id = entity.Id;
            document.CreatedOn = entity.CreatedOn;
            document.UpdatedOn = entity.UpdatedOn;
            document.UserId = entity.UserId;
            document.Status = entity.Status;
            document.Name = GetPostName(document);
            document.Description = GetPostDescription(document);

            /*
            document.Content = entity.Content;
            document.UserId = entity.UserId;
            */
            /*
            document.ThreadId = entity.ThreadId;
            document.ShortThreadId = Foundations.Common.Extensions.GuidExtensions.ShortenBase64(entity.ThreadId);
            document.PostType = entity.Type;
            
             * TODO: Move these into subdocument updates
            document.threadId = entity.threadId;
            document.ParentId = entity.ParentId;
            document.ParentIds = entity.ParentIdItems?.ToList();
            document.channelId = entity.channelId;

            
            document.Tags = entity.TagItems;
            document.Category = entity.Category;
            document.Profile = profile ?? 
                new ContentPostUserSubdocument()
                {
                    Username = "Anonymous"
                };
            */
        }
        public static void HydrateFrom(this ContentPostPinEntity entity, ContentPostPinEntity from)
        {
            entity.UserId = from.UserId;
            entity.CreatedOn = from.CreatedOn;
            entity.UpdatedOn = from.UpdatedOn;
            entity.ContentPostId = from.ContentPostId;
            entity.PinnedContentPostId = from.PinnedContentPostId;
            entity.Reason = from.Reason;
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

        public static void Hydrate(this ContentFeedEntity entity, ContentFeedDocument document)
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
        public static void Hydrate(this ContentChatEntity entity, ContentChatDocument document)
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

        public static void Hydrate(this ContentListEntity entity, ContentListDocument document)
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

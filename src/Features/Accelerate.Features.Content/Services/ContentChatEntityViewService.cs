using Accelerate.Features.Content.Models.UI;
using Accelerate.Features.Content.Models.Views;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Content.Models.View;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch;

namespace Accelerate.Features.Content.Services
{
    public class ContentChatEntityViewService : BaseContentEntityViewService<ContentChatDocument>
    {
        public ContentChatEntityViewService(
            IMetaContentService metaContent, 
            IContentViewSearchService viewSearchService, 
            IElasticService<ContentChatDocument> searchService) 
            : base(metaContent, viewSearchService, searchService)
        {
            EntityName = "Chat";
        }
        public override ContentChatPage CreateIndexPage(AccountUser user, SearchResponse<ContentChatDocument> items, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var model = base.CreateIndexPage(user, items, aggregateResponse);
            var viewModel = new ContentChatPage(model);
            viewModel.Test = "INDEX";
            return viewModel;
        }
        public override ContentChatPage CreateEntityPage(AccountUser user, ContentChatDocument item, SearchResponse<ContentChatDocument> items, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var model = base.CreateEntityPage(user, item, items, aggregateResponse);
            var viewModel = new ContentChatPage(model);
            viewModel.FormCreatePost = this.CreatePostForm(user, PostbackType.POST, null, item);
            viewModel.Test = "ENTITY";
            return viewModel;
        }
        private FormField FormFieldChat(Guid? chatId)
        {
            return new FormField()
            {
                Name = "ChatId",
                FieldType = FormFieldTypes.input,
                Hidden = true,
                Disabled = true,
                AriaInvalid = false,
                Value = chatId,
            };
        }
        public override ContentSubmitForm CreatePostForm(AccountUser user, PostbackType type = PostbackType.POST, ContentPostViewDocument item = null, ContentChatDocument doc = null)
        {
            var model = base.CreatePostForm(user, type, item);

            if(doc != null )
            {
                model.Fields.Add(FormFieldChat(doc.Id));
            }
            
            return model;
        }
    }
}

using Accelerate.Features.Content.Models.UI;
using Accelerate.Features.Content.Models.Views;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.UI.Components.Table;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Content.Models.View;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch;

namespace Accelerate.Features.Content.Services
{
    public class ContentChannelEntityViewService : BaseContentEntityViewService<ContentChannelDocument>
    {
        public ContentChannelEntityViewService(
            IMetaContentService metaContent, 
            IContentViewSearchService viewSearchService, 
            IElasticService<ContentChannelDocument> searchService) 
            : base(metaContent, viewSearchService, searchService)
        {
            EntityName = "Channel";
        }
        public override ContentChannelPage CreateIndexPage(AccountUser user, SearchResponse<ContentChannelDocument> items, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var model = base.CreateIndexPage(user, items, aggregateResponse);
            var viewModel = new ContentChannelPage(model);
            viewModel.Test = "INDEX";
            viewModel.Listing = new AclAjaxListing<ContentChannelDocument>()
            {
                Items = items.Documents.ToList()
            };
            return viewModel;
        }
        public override ContentChannelPage CreateEntityPage(AccountUser user, ContentChannelDocument item, SearchResponse<ContentChannelDocument> items, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var model = base.CreateEntityPage(user, item, items, aggregateResponse);
            var viewModel = new ContentChannelPage(model);
            viewModel.FormCreatePost = this.CreatePostForm(user, PostbackType.POST, null, item);
            viewModel.Test = "ENTITY";
            return viewModel;
        }
        private FormField FormFieldChannel(Guid? listId)
        {
            return new FormField()
            {
                Name = "ChannelId",
                FieldType = FormFieldTypes.input,
                Hidden = true,
                Disabled = true,
                AriaInvalid = false,
                Value = listId,
            };
        }
        public override ContentSubmitForm CreatePostForm(AccountUser user, PostbackType type = PostbackType.POST, ContentPostViewDocument item = null, ContentChannelDocument doc = null)
        {
            var model = base.CreatePostForm(user, type, item);

            if (doc != null)
            {
                model.Fields.Add(FormFieldChannel(doc.Id));
            }

            return model;
        }
        public override ContentBasePage CreateAllPage(AccountUser user, SearchResponse<ContentChannelDocument> items, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var model = base.CreateAllPage(user, items, aggregateResponse);
            var viewModel = new ContentChannelPage(model);
            viewModel.Listing = new Foundations.Common.Models.UI.Components.Table.AclAjaxListing<ContentChannelDocument>()
            {
                Items = items.Documents.ToList()
            };
            return viewModel;
        }
    }
}

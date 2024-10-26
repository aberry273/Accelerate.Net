using Accelerate.Features.Content.Models.UI;
using Accelerate.Features.Content.Models.Views;
using Accelerate.Foundations.Users.Models.Entities;
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
        public override ContentChannelPage CreateIndexPage(UsersUser user, SearchResponse<ContentChannelDocument> items, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var model = base.CreateIndexPage(user, items, aggregateResponse);
            var viewModel = new ContentChannelPage(model);
            viewModel.Test = "INDEX";
            viewModel.Listing = new AclAjaxListing<AclCard>()
            {
                Items = items.Documents.Select(CreateCardFromContent).ToList()
            };
            return viewModel;
        }
        public override async Task<ContentBasePage> CreateEntityPage(UsersUser user, ContentChannelDocument item, SearchResponse<ContentChannelDocument> items, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var model = await base.CreateEntityPage(user, item, items, aggregateResponse);
            var viewModel = new ContentChannelPage(model);
            viewModel.FormCreatePost = this.CreatePostForm(user, null, item);
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
        public override ContentSubmitForm CreatePostForm(UsersUser user,  ContentPostViewDocument item = null, ContentChannelDocument doc = null)
        {
            var model = base.CreatePostForm(user, item);

            if (doc != null)
            {
                model.Fields.Add(FormFieldChannel(doc.Id));
            }

            return model;
        }
        public override async Task<ContentBasePage> CreateAllPage(UsersUser user, SearchResponse<ContentChannelDocument> items, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var model = await base.CreateAllPage(user, items, aggregateResponse);
            var viewModel = new ContentChannelPage(model);
            viewModel.Listing = new AclAjaxListing<AclCard>()
            {
                Items = items.IsSuccess()
                    ? items.Documents.Select(CreateCardFromContent).ToList()
                    : new List<AclCard>()
            };
            return viewModel;
        }
    }
}

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
    public class ContentListEntityViewService : BaseContentEntityViewService<ContentListDocument>
    {
        public ContentListEntityViewService(
            IMetaContentService metaContent, 
            IContentViewSearchService viewSearchService, 
            IElasticService<ContentListDocument> searchService) 
            : base(metaContent, viewSearchService, searchService)
        {
            EntityName = "List";
        }
        public override ContentListPage CreateIndexPage(AccountUser user, SearchResponse<ContentListDocument> items, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var model = base.CreateIndexPage(user, items, aggregateResponse);
            var viewModel = new ContentListPage(model);
            viewModel.Test = "INDEX";
            return viewModel;
        }
        public override async Task<ContentBasePage> CreateEntityPage(AccountUser user, ContentListDocument item, SearchResponse<ContentListDocument> items, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var model = await base.CreateEntityPage(user, item, items, aggregateResponse);
            var viewModel = new ContentListPage(model);
            viewModel.FormCreatePost = this.CreatePostForm(user, null, item);
            viewModel.Test = "ENTITY"; 

            return viewModel;
        }
        private FormField FormFieldList(Guid? listId)
        {
            return new FormField()
            {
                Name = "ListId",
                FieldType = FormFieldTypes.input,
                Hidden = true,
                Disabled = true,
                AriaInvalid = false,
                Value = listId,
            };
        }
        public override ContentSubmitForm CreatePostForm(AccountUser user, ContentPostViewDocument item = null, ContentListDocument doc = null)
        {
            var model = base.CreatePostForm(user, item);

            if (doc != null)
            {
                model.Fields.Add(FormFieldList(doc.Id));
            }

            return model;
        }
        public override async Task<ContentBasePage> CreateAllPage(AccountUser user, SearchResponse<ContentListDocument> items, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var model = await base.CreateAllPage(user, items, aggregateResponse);
            var viewModel = new ContentListPage(model);
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

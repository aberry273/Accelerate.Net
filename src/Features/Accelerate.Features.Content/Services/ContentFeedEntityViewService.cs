using Accelerate.Features.Content.Models.Views;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch;

namespace Accelerate.Features.Content.Services
{
    public class ContentFeedEntityViewService : BaseContentEntityViewService<ContentFeedDocument>
    {
        public ContentFeedEntityViewService(
            IMetaContentService metaContent, 
            IContentViewSearchService viewSearchService, 
            IElasticService<ContentFeedDocument> searchService) 
            : base(metaContent, viewSearchService, searchService)
        {
            EntityName = "Feed";
        }
        public override ContentFeedPage CreateIndexPage(AccountUser user, SearchResponse<ContentFeedDocument> items, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var model = base.CreateIndexPage(user, items, aggregateResponse);
            var viewModel = new ContentFeedPage(model);
            viewModel.Test = "INDEX";
            return viewModel;
        }
        public override ContentFeedPage CreateEntityPage(AccountUser user, ContentFeedDocument item, SearchResponse<ContentFeedDocument> items, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var model = base.CreateEntityPage(user, item, items, aggregateResponse);
            var viewModel = new ContentFeedPage(model);
            viewModel.Test = "ENTITY";
            return viewModel;
        }
    }
}

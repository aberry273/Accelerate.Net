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
            viewModel.PostsApiUrl = $"{viewModel.PostsApiUrl}/feed";

            return viewModel;
        }
        public override async Task<ContentBasePage> CreateEntityPage(AccountUser user, ContentFeedDocument item, SearchResponse<ContentFeedDocument> items, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var model = await base.CreateEntityPage(user, item, items, aggregateResponse);
            var viewModel = new ContentFeedPage(model);
            viewModel.Test = "ENTITY";
            viewModel.PostsApiUrl = $"{viewModel.PostsApiUrl}/feed";

            viewModel.ModalCreateReply = CreateModalCreateReplyForm(user, null);

            return viewModel;
        }
        public override async Task<ContentBasePage> CreateAllPage(AccountUser user, SearchResponse<ContentFeedDocument> items, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var model = await base.CreateAllPage(user, items, aggregateResponse);
            var viewModel = new ContentFeedPage(model);
            viewModel.UserListing = new AclAjaxListing<AclCard>()
            {
                Items = items.IsSuccess()
                    ? items.Documents.Where(x => x.UserId == user.Id).Select(CreateCardFromContent).ToList()
                    : new List<AclCard>()
            };

            viewModel.AllListing = new AclAjaxListing<AclCard>()
            {
                Items = items.IsSuccess()
                    ? items.Documents.Where(x => x.UserId != user.Id).Select(CreateCardFromContent).ToList()
                    : new List<AclCard>()
            };
            viewModel.PostsApiUrl = $"{viewModel.PostsApiUrl}/feed";
           
            return viewModel;
        }
        public ModalCreateContentPostReply CreateModalCreateReplyForm(AccountUser user, ContentPostViewDocument? post)
        {
            var model = new ModalCreateContentPostReply();
            model.UserId = user.Id;
            model.Title = "Reply to post";
            model.Text = "Test form text";
            model.IsAuthenticated = user != null;
            model.Event = "on:feed:open";
            model.ApiUrl = "/api/contentsearch/posts";
            //model.Item = post;
            model.Form = new SocialPostFormModel()
            {
                Form = CreateReplyForm(user, post),
                Actions = new List<object>(),
                FormatActions = new List<string>()
            };
            return model;
        }
    }
}

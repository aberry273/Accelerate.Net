using Accelerate.Features.Admin.Models.Views;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Elastic.Clients.Elasticsearch;

namespace Accelerate.Features.Admin.Services
{
    public class AdminViewService : IAdminViewService
    {
        IMetaContentService _metaContentService;
        public AdminViewService(IMetaContentService metaContent)
        {
            _metaContentService = metaContent;
        }

        private AdminBasePage CreateBaseContent(AccountUser user)
        {
            var profile = Accelerate.Foundations.Account.Helpers.AccountHelpers.CreateUserProfile(user);
            var baseModel = _metaContentService.CreatePageBaseContent(profile);
            var viewModel = new AdminBasePage(baseModel);
            return viewModel;
        }
        public AdminBasePage CreateJobsPage(AccountUser user)
        {
            var model = CreateBaseContent(user);
            var viewModel = model;

            var sectionName = "Thread";
            var pageName = "All";
            viewModel.SideNavigation.Selected = $"{sectionName}s";
            //viewModel.PageLinks = CreatePageNavigationGroup(sectionName, pageName);
            //viewModel.PageLinks.Items.AddRange(GetThreadLinks(posts));
            //viewModel.PageActions = CreatePageActionsGroup(sectionName, pageName);

            viewModel.UserId = user != null ? user.Id : null;
            //viewModel.FormCreatePost = user != null ? CreatePostForm(user) : null;
            //viewModel.ModalCreateLabel = CreateModalChannelForm(user);
            //viewModel.ModalEditReply = CreateModalEditReplyForm(user);
            //viewModel.ModalDeleteReply = CreateModalDeleteReplyForm(user);
            viewModel.ActionsApiUrl = "/api/contentpostactions";
            viewModel.PostsApiUrl = "/api/contentsearch/posts";
            viewModel.FilterEvent = "filter:update";
            viewModel.ActionEvent = "action:post";
            return viewModel;
        }
    }
}

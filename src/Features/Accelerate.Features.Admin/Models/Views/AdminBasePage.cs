using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;

namespace Accelerate.Features.Admin.Models.Views
{

    public class AdminBasePage : BasePage
    {
        public AdminBasePage(AdminBasePage model) : base(model)
        {
            RouteName = model.RouteName;
            ParentUrl = model.ParentUrl;
            FilterEvent = model.FilterEvent;
            ActionEvent = model.ActionEvent;
            ActionsApiUrl = model.ActionsApiUrl;
            PostsApiUrl = model.PostsApiUrl;
            Id = model.Id;
            ModalDelete = model.ModalDelete;
            PageActions = model.PageActions;
            PageLinks = model.PageLinks;
            //FormCreatePost = model.FormCreatePost;
            ModalEditChannel = model.ModalEditChannel;
            ModalDeleteChannel = model.ModalDeleteChannel;
            Filters = model.Filters;
        }
        public AdminBasePage(BasePage model) : base(model)
        {
        }
        public string RouteName { get; set; }
        public string ParentUrl { get; set; }
        public string FilterEvent { get; set; }
        public string ActionEvent { get; set; }
        public string ActionsApiUrl { get; set; }
        public string PostsApiUrl { get; set; }

        public Guid Id { get; set; }

        public ModalForm ModalDelete { get; set; }
        public ButtonGroup PageActions { get; set; }
        public NavigationGroup PageLinks { get; set; }
        ///public AjaxForm FormCreatePost { get; set; }
        public ModalForm ModalEditChannel { get; set; }
        public ModalForm ModalDeleteChannel { get; set; }
        public NavigationFilter Filters { get; set; }
    }
}

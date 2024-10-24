using Accelerate.Features.Content.Models.Data;
using Accelerate.Features.Content.Models.UI;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.UI.Components.Table;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Content.Models.Data;

namespace Accelerate.Features.Content.Models.Views
{
    public class ContentFeedPage : ContentBasePage
    {
        public ContentFeedPage(ContentBasePage model) : base(model)
        {/*
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
            FormCreatePost = model.FormCreatePost;
            ModalEditChannel = model.ModalEditChannel;
            ModalDeleteChannel = model.ModalDeleteChannel;
            Filters = model.Filters;
            */
        }
        public AclAjaxListing<AclCard> AllListing { get; set; }
        public AclAjaxListing<AclCard> UserListing { get; set; }
        public ModalCreateContentPostReply ModalCreateReply { get; set; }
        public string Test { get; set; }
    }
}

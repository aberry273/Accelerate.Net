using Accelerate.Features.Content.Models.Data;
using Accelerate.Features.Content.Models.UI;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Content.Models.View;

namespace Accelerate.Features.Content.Models.Views
{
    public class ContentPostPage : ContentBasePage
    {
        public Guid UserId { get; set; }
        public ContentPostViewDocument Item { get; set; }
        public List<ContentPostViewDocument> Thread { get; set; }

        /*
        public List<ContentPostActionsDocument> Actions { get; set; } = new List<ContentPostActionsDocument>();
        public List<ContentPostDocument> Quotes { get; set; } = new List<ContentPostDocument>();
        public List<ContentPostDocument> Parents { get; set; } = new List<ContentPostDocument>();
        */
        public List<ContentPostViewDocument> Replies { get; set; } = new List<ContentPostViewDocument>();
        public string FilterEvent { get; set; } = "filter:update";
        public string ActionEvent { get; set; } = "action:post";
        public string ActionsApiUrl { get; set; }
        public string PostsApiUrl { get; set; }
        public string PinnedPostsApiUrl { get; set; }
        public string ParentPostsApiUrl { get; set; }
        public NavigationItem ParentLink { get; set; }
        public NavigationItem ChannelLink { get; set; }
        public NavigationFilter Filters { get; set; } 

        public ModalCreateContentPostReply ModalCreateReply { get; set; }

        public ModalForm ModalDeleteReply { get; set; }
        public ModalForm ModalPinReply { get; set; }
        public ModalForm ModalLabelReply { get; set; }
        public AclAjaxListing<ContentPostViewDocument> Listing { get; set; }
        public string Test { get; set; }
        public ContentPostPage(ContentBasePage model) : base(model)
        {

        }
    }
}

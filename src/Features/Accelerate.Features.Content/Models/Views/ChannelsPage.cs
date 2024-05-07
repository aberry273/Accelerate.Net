using Accelerate.Features.Content.Models.Data;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Content.Models.Data;

namespace Accelerate.Features.Content.Models.Views
{
    public class ChannelsPage : BasePage
    {
        public Guid UserId { get; set; }
        public NavigationGroup ChannelsDropdown { get; set; }
        public AjaxForm FormCreatePost { get; set; }
        public ModalForm ModalCreateChannel { get; set; }
        public ModalForm ModalEditReply { get; set; }
        public ModalForm ModalDeleteReply { get; set; }
        public List<NavigationFilter> Filters { get; set; } = new List<NavigationFilter>();
        public string FilterEvent { get; set; } = "filter:update";
        public string ActionEvent { get; set; } = "action:post";
        public ChannelsPage(BasePage model) : base(model)
        {
        }
    }
}

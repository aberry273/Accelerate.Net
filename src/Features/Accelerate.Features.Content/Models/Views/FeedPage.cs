using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;

namespace Accelerate.Features.Content.Models.Views
{
    public class FeedPage : BasePage
    {
        public Guid UserId { get; set; }
        public AjaxForm FormCreateReply { get; set; }
        public ModalForm ModalEditReply { get; set; }
        public ModalForm ModalDeleteReply { get; set; }
        public FeedPage(BasePage model) : base(model)
        {

        }
    }
}

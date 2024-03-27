using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Content.Models;

namespace Accelerate.Features.Content.Models.Views
{
    public class ThreadPage : BasePage
    {
        public Guid UserId { get; set; }
        public ContentPostEntity Item { get; set; }
        public List<ContentPostEntity> Replies { get; set; } = new List<ContentPostEntity>();
        public string PreviousUrl { get; set; }
        public AjaxForm FormCreateReply { get; set; }
        public ModalForm ModalEditReply { get; set; }
        public ModalForm ModalDeleteReply { get; set; }
        public ThreadPage(BasePage model) : base(model)
        {

        }
    }
}

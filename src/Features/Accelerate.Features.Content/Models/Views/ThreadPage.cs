using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;

namespace Accelerate.Features.Content.Models.Views
{
    public class ThreadPage : BasePage
    {
        public Guid UserId { get; set; }
        public ContentPostDocument Item { get; set; }
        public List<ContentPostDocument> Replies { get; set; } = new List<ContentPostDocument>();
        public string PreviousUrl { get; set; }
        public AjaxForm FormCreateReply { get; set; }
        public ModalForm ModalEditReply { get; set; }
        public ModalForm ModalDeleteReply { get; set; }
        public ThreadPage(BasePage model) : base(model)
        {

        }
    }
}

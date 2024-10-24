using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.View;

namespace Accelerate.Features.Content.Models.UI
{
    public class ModalCreateContentPostReply : ModalForm
    {
        public Guid UserId { get; set; }
        public string ApiUrl { get; set; }
        public bool IsAuthenticated { get; set; }
        public ContentPostViewDocument Item { get; set; }
        public List<AclButtonAction> Actions { get; set; }
        public new SocialPostFormModel Form { get; set; }
    }
}

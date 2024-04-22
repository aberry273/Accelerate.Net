using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;

namespace Accelerate.Features.Account.Models.Views
{
    public class ManagePage : BasePage
    {
        public AjaxForm ProfileImageForm { get; set; }
        public AjaxForm ProfileForm { get; set; }
        public ModalForm ModalCreateMedia { get; set; }
        public Guid UserId { get; set; }
        public List<NavigationItem> Tabs { get; set; }
        public ManagePage(BasePage model) : base(model)
        {

        }
    }
}

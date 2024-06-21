using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;

namespace Accelerate.Features.Account.Models.Views
{
    public class ManagePage : BasePage
    {
        public string ActionUrl { get; set; }
        public string SearchUrl { get; set; }
        public string FilterEvent { get; set; }
        public AjaxForm ProfileImageForm { get; set; }
        public AjaxForm ProfileForm { get; set; }
        public ModalForm ModalCreateMedia { get; set; }
        public Guid UserId { get; set; }
        public List<NavigationItem> Tabs { get; set; }
        public List<NavigationFilter> Filters { get; set; } = new List<NavigationFilter>();
        public ManagePage(BasePage model) : base(model)
        {

        }
    }
}

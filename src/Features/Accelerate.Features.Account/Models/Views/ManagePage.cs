using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;

namespace Accelerate.Features.Account.Models.Views
{
    public class ManagePage : BasePage
    {
        public AccountUserStatus UserStatus { get; set; }
        public string ActionUrl { get; set; }
        public string SearchUrl { get; set; }
        public string FilterEvent { get; set; }
        public AjaxForm DeactivateForm { get; set; }
        public AjaxForm DeleteForm { get; set; }
        public AjaxForm ReactivateForm { get; set; }
        public AjaxForm UserForm { get; set; }
        public AjaxForm ProfileImageForm { get; set; }
        public AjaxForm ProfileForm { get; set; }
        public ModalForm ModalCreateMedia { get; set; }
        public Guid UserId { get; set; }
        public List<NavigationItem> Tabs { get; set; }
        public NavigationFilter Filters { get; set; }
        public ManagePage(BasePage model) : base(model)
        {

        }
    }
}

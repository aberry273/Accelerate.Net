using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;

namespace Accelerate.Features.Account.Models.Views
{
    public class ManagePage : BasePage
    {
        public AjaxForm ProfileImageForm { get; set; }
        public AjaxForm ProfileForm { get; set; }
        public Guid UserId { get; set; }
        public ManagePage(BasePage model) : base(model)
        {

        }
    }
}

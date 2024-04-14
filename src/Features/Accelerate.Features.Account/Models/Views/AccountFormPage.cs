using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;

namespace Accelerate.Features.Account.Models.Views
{
    public class AccountFormPage : BasePage
    {
        public string Title { get; set; }
        public Form Form { get; set; } = new Form();
        public List<NavigationItem> Links { get; set; } = new List<NavigationItem>();
        public AccountFormPage(BasePage model) : base(model)
        {

        }
    }
}

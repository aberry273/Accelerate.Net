using Accelerate.Features.Accounts.Models.Views;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;

namespace Accelerate.Features.Accounts.Models.Views
{
    public class AccountsNotFoundPage : BasePage
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public NavigationItem ReturnLink { get; set; }
        public AccountsNotFoundPage(BasePage model) : base(model)
        {

        }
    }
}

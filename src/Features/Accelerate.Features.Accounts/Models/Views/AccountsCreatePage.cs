
using Accelerate.Features.Accounts.Models.Views;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Database.Models;

namespace Accelerate.Features.Accounts.Models.Views
{
    public class AccountsCreatePage : AccountsBasePage
    {
        public string RedirectRoute { get; set; }
        public AjaxForm Form { get; set; }
        public AccountsCreatePage(AccountsBasePage model) : base(model)
        {
        }
    }
}

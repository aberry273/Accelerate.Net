
using Accelerate.Features.Accounts.Models.Views;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Database.Models;

namespace Accelerate.Features.Accounts.Models.Views
{
    public class AccountsFormPage : AccountsBasePage
    {
        public string RedirectRoute { get; set; }
        public AjaxForm Form { get; set; }
        public AccountsFormPage(AccountsBasePage model) : base(model)
        {
        }
    }
}

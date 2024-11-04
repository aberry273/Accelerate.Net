using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Common.Models.UI.Components;

namespace Accelerate.Features.Accounts.Models.Views
{
    public class AccountsBusinessAccountPage : AccountsBasePage<AccountsBusinessEntity>
    {
        public AccountsBusinessAccountPage(AccountsBasePage<AccountsBusinessEntity> model) : base(model)
        {
        }
        public AjaxForm? AccountForm { get; set; }
        public AjaxForm? RegisteredAddressForm { get; set; }
        public AjaxForm? OperatingAddressForm { get; set; }
        public AjaxForm? PrimaryContactForm { get; set; }
    }
}

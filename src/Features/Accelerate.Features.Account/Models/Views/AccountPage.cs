using Accelerate.Foundations.Common.Models.Views;

namespace Accelerate.Features.Account.Models.Views
{
    public class AccountPage : BasePage
    {
        public ConfirmForm Form { get; set; } = new ConfirmForm();
        public AccountPage(BasePage model) : base(model)
        {

        }
    }
}

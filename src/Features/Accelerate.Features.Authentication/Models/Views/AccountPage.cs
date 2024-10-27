using Accelerate.Foundations.Common.Models.Views;

namespace Accelerate.Features.Authentication.Models.Views
{
    public class AccountPage : BasePage
    {
        public ConfirmForm Form { get; set; } = new ConfirmForm();
        public AccountPage(BasePage model) : base(model)
        {

        }
    }
}

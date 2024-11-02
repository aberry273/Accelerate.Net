using Accelerate.Foundations.Common.Models.Views;

namespace Accelerate.Features.Account.Models.Views
{
    public class RegisterPage : BasePage
    {
        public RegisterForm Form { get; set; } = new RegisterForm();
        public List<NavigationItem> Links { get; set; } = new List<NavigationItem>();
        public RegisterPage(BasePage model) : base(model)
        {

        }
    }
}

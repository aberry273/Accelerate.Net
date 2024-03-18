using Accelerate.Foundations.Common.Models;

namespace Accelerate.Features.Account.Models.Views
{
    public class LoginPage : BasePage
    {
        public LoginForm Form { get; set; } = new LoginForm();
        public LoginPage(BasePage model) : base(model)
        {

        }
    }
}

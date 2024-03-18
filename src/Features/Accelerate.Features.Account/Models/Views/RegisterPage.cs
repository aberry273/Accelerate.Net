using Accelerate.Foundations.Common.Models;

namespace Accelerate.Features.Account.Models.Views
{
    public class RegisterPage : BasePage
    {
        public RegisterForm Form { get; set; } = new RegisterForm();
        public RegisterPage(BasePage model) : base(model)
        {

        }
    }
}

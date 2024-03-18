using Accelerate.Foundations.Common.Models;

namespace Accelerate.Features.Account.Models.Views
{
    public class ManagePage : BasePage
    {
        public ProfileForm Form { get; set; } = new ProfileForm();
        public ManagePage(BasePage model) : base(model)
        {

        }
    }
}

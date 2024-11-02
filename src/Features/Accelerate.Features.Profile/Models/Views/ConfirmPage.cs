using Accelerate.Foundations.Common.Models.Views;

namespace Accelerate.Features.Profile.Models.Views
{
    public class ConfirmPage : BasePage
    {
        public ConfirmForm Form { get; set; } = new ConfirmForm();
        public ConfirmPage(BasePage model) : base(model)
        {

        }
    }
}

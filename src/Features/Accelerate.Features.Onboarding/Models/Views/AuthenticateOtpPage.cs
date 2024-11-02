using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;

namespace Accelerate.Features.Onboarding.Models.Views
{

    public class AuthenticateOtpPage : OnboardingBasePage
    {
        public AuthenticateOtpPage(OnboardingBasePage model) : base(model)
        {
            Id = model.Id;
            Steps = model.Steps;
        }
        public AuthenticateOtpPage(BasePage model) : base(model)
        {
        }
        public AjaxForm ResendForm { get; set; }
    }
}

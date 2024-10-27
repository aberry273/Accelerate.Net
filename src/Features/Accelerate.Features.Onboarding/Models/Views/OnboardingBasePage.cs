using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;

namespace Accelerate.Features.Onboarding.Models.Views
{

    public class OnboardingBasePage : BasePage
    {
        public OnboardingBasePage(OnboardingBasePage model) : base(model)
        {
            Id = model.Id;
        }
        public OnboardingBasePage(BasePage model) : base(model)
        {
        }
        public Guid Id { get; set; }
        public Form Form { get; set; }
    }
}

using Accelerate.Features.Rates.Models.Views;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;

namespace Accelerate.Features.Rates.Models.Views
{
    public class RatesNotFoundPage : BasePage
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public NavigationItem ReturnLink { get; set; }
        public RatesNotFoundPage(BasePage model) : base(model)
        {

        }
    }
}

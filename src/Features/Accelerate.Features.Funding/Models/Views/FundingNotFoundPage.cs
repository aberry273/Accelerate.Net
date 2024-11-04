using Accelerate.Features.Funding.Models.Views;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;

namespace Accelerate.Features.Funding.Models.Views
{
    public class FundingNotFoundPage : BasePage
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public NavigationItem ReturnLink { get; set; }
        public FundingNotFoundPage(BasePage model) : base(model)
        {

        }
    }
}

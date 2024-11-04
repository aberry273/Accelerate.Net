
using Accelerate.Features.Funding.Models.Views;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Database.Models;

namespace Accelerate.Features.Funding.Models.Views
{
    public class FundingFormPage : FundingBasePage
    {
        public string RedirectRoute { get; set; }
        public AjaxForm Form { get; set; }
        public FundingFormPage(FundingBasePage model) : base(model)
        {
        }
    }
}

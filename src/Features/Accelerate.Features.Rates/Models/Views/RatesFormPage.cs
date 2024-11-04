
using Accelerate.Features.Rates.Models.Views;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Database.Models;

namespace Accelerate.Features.Rates.Models.Views
{
    public class RatesFormPage : RatesBasePage
    {
        public string RedirectRoute { get; set; }
        public AjaxForm Form { get; set; }
        public RatesFormPage(RatesBasePage model) : base(model)
        {
        }
    }
}

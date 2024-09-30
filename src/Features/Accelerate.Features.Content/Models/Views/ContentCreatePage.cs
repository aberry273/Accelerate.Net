using Accelerate.Features.Content.Models.Data;
using Accelerate.Features.Content.Models.UI;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Content.Models.Data;

namespace Accelerate.Features.Content.Models.Views
{
    public class ContentCreatePage : ContentBasePage
    {
        public string RedirectRoute { get; set; }
        public AjaxForm Form { get; set; }
        public ContentCreatePage(ContentBasePage model) : base(model)
        {
        }
    }
}

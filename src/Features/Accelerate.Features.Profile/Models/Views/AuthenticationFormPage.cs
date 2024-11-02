using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;

namespace Accelerate.Features.Profile.Models.Views
{
    public struct ProviderLink
    {
        public string Name { get; set; }
        public string Color { get; set; }
    }
    public class ProfileFormPage : BasePage
    {
        public string Title { get; set; }
        public Form Form { get; set; } = new Form();

        public string Message { get; set; }

        public string ExternalLoginAction { get; set; }
        public string ExternalLoginPostbackUrl { get; set; }
        public IEnumerable<ProviderLink> Providers { get; set; }
        public List<NavigationItem> Links { get; set; } = new List<NavigationItem>();
        
        public ProfileFormPage(BasePage model) : base(model)
        {

        }
    }
}

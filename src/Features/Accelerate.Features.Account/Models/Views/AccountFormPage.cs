using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Microsoft.AspNetCore.Authentication;

namespace Accelerate.Features.Account.Models.Views
{
    public class AccountFormPage : BasePage
    {
        public string Title { get; set; }
        public Form Form { get; set; } = new Form();

        public string Message { get; set; }

        public string ExternalLoginAction { get; set; }
        public string ExternalLoginPostbackUrl { get; set; }
        public IEnumerable<AuthenticationScheme> Providers { get; set; }
        public List<NavigationItem> Links { get; set; } = new List<NavigationItem>();
        
        public AccountFormPage(BasePage model) : base(model)
        {

        }
    }
}

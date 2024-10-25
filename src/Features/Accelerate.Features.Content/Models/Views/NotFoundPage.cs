using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;

namespace Accelerate.Features.Content.Models.Views
{
    public class NotFoundPage : BasePage
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public NavigationItem ReturnLink { get; set; }
        public NotFoundPage(BasePage model) : base(model)
        {

        }
    }
}

using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;

namespace Accelerate.Features.Admin.Models.Views
{
    public class NotFoundPage : AdminBasePage
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public NavigationItem ReturnLink { get; set; }
        public NotFoundPage(AdminBasePage model) : base(model)
        {

        }
    }
}

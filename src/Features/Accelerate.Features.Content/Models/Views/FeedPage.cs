using Accelerate.Foundations.Common.Models;

namespace Accelerate.Features.Content.Models.Views
{
    public class FeedPage : BasePage
    {
        public Guid UserId { get; set; }
        public FeedPage(BasePage model) : base(model)
        {

        }
    }
}

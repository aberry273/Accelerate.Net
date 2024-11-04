using Accelerate.Features.Transactions.Models.Views;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;

namespace Accelerate.Features.Transactions.Models.Views
{
    public class TransactionsNotFoundPage : BasePage
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public NavigationItem ReturnLink { get; set; }
        public TransactionsNotFoundPage(BasePage model) : base(model)
        {

        }
    }
}

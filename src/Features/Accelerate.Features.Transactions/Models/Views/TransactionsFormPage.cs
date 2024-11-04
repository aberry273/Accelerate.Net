
using Accelerate.Features.Transactions.Models.Views;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Database.Models;

namespace Accelerate.Features.Transactions.Models.Views
{
    public class TransactionsFormPage : TransactionsBasePage
    {
        public string RedirectRoute { get; set; }
        public AjaxForm Form { get; set; }
        public TransactionsFormPage(TransactionsBasePage model) : base(model)
        {
        }
    }
}

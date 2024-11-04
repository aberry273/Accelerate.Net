using Accelerate.Foundations.Transactions.Models.Entities;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Integrations.AlphaVantage.Models.Data;

namespace Accelerate.Features.Transactions.Models.Views
{
    public class TransactionsTransactionPage : TransactionsBasePage<TransactionsTransactionEntity>
    {
        public TransactionsTransactionPage(TransactionsBasePage<TransactionsTransactionEntity> model) : base(model)
        {
        }
        public AjaxForm? RegisteredAddress { get; set; }
    }
}

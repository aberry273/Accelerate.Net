using Accelerate.Foundations.Funding.Models.Entities;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Integrations.AlphaVantage.Models.Data;

namespace Accelerate.Features.Funding.Models.Views
{
    public class FundingBankAccountPage : FundingBasePage<FundingBankAccountEntity>
    {
        public FundingBankAccountPage(FundingBasePage<FundingBankAccountEntity> model) : base(model)
        {
        }
        public AjaxForm? RegisteredAddress { get; set; }
    }
}

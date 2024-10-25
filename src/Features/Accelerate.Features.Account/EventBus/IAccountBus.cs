using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.EventPipelines.EventBus;

namespace Accelerate.Features.Content.EventBus
{
    public interface IAccountBus : IDataBus<AccountUser>
    {
    }
}

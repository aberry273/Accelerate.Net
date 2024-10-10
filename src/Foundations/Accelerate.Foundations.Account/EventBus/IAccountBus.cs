using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.EventPipelines.EventBus;

namespace Accelerate.Foundations.Account.EventBus
{
    public interface IAccountBus : IDataBus<AccountUser>
    {
    }
}

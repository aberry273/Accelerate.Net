using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.EventPipelines.EventBus;

namespace Accelerate.Foundations.Users.EventBus
{
    public interface IUsersBus : IDataBus<UsersUser>
    {
    }
}

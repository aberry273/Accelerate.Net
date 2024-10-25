using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Websockets.Hubs
{
    public interface IBaseHubClient<T>
    {
        Task SendMessage(string user, T data);
    }
}

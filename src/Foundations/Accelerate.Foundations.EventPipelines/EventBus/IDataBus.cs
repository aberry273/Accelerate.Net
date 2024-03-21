using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.EventPipelines.EventBus
{
    public interface IDataBus<T> : IBus
    {
    }
}

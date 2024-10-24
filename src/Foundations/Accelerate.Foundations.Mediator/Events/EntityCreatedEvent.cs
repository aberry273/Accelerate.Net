using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Mediator.Events
{
    public class EntityCreatedEvent<T> : BaseEvent
    {
        public T Entity { get; set; }
    }
}

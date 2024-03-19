using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Services.InMemoryEventBus
{
    public sealed class InMemoryEventBusService
    {
        public InMemoryEventBusService()
        {
        }
    }

    readonly record struct Joke(string Setup, string Punchline);
}

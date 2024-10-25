using Accelerate.Foundations.Integrations.Contracts;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Integrations.MassTransit.Producers
{
    public class GettingStartedProducer
    {
        string _serviceAddress = "loopback://localhost/GGPC_AccelerateServicesInMemoryEventBus_bus_6ngoyyyfbayx9nz4bdqroo6tdn\r\n";
        string targetAddress = "queue:process-input-item";
        public async Task SendOrder(ISendEndpointProvider sendEndpointProvider)
        {
            var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:process-input-item"));

            await endpoint.Send(new GettingStarted { Value = "Producter value" });
        }
    }
}

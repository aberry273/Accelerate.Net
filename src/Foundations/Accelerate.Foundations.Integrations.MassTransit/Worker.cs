using Accelerate.Foundations.Integrations.Contracts;
using global::MassTransit;
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace Accelerate.Foundations.Integrations.MassTransit
{

    public class Worker : BackgroundService
    {
        readonly IBus _bus;

        public Worker(IBus bus)
        {
            _bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _bus.Publish(new GettingStarted { Value = $"The time is {DateTimeOffset.Now}" }, stoppingToken);

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}

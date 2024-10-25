using Accelerate.Foundations.Integrations.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Integrations.MassTransit.Consumers
{
    public class GettingStartedConsumer : IConsumer<GettingStarted>
    {
        readonly ILogger<GettingStartedConsumer> _logger;

        public GettingStartedConsumer(ILogger<GettingStartedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<GettingStarted> context)
        {
            _logger.LogInformation("Received Text: {Text}", context.Message.Value);

            return Task.CompletedTask;
        }
    }
}

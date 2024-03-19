using Accelerate.Foundations.Integrations.Contracts;
using MassTransit;

namespace Accelerate.Services.InMemoryEventBus
{
    public sealed class WindowsBackgroundService(
    InMemoryEventBusService eventBusService,
    IBus bus,
    ILogger<WindowsBackgroundService> logger) : BackgroundService
    {
        readonly IBus _bus = bus;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    //Listen for events
                    //Run publisher pipelines for events


                    //string joke = eventBusService.GetJoke();
                    // logger.LogWarning("{Joke}", joke);
                    logger.LogWarning("{Address}", _bus.Address);
                    await _bus.Publish(new GettingStarted { Value = $"The time is {DateTimeOffset.Now}" }, stoppingToken);

                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // When the stopping token is canceled, for example, a call made from services.msc,
                // we shouldn't exit with a non-zero exit code. In other words, this is expected...
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{Message}", ex.Message);

                // Terminates this process and returns an exit code to the operating system.
                // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
                // performs one of two scenarios:
                // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
                // 2. When set to "StopHost": will cleanly stop the host, and log errors.
                //
                // In order for the Windows Service Management system to leverage configured
                // recovery options, we need to terminate the process with a non-zero exit code.
                Environment.Exit(1);
            }
        }
    }
}

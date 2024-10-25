using Accelerate.Services.InMemoryEventBus;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "Accelerate.Services.InMemoryEventBus Service";
});

LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(builder.Services);

builder.Services.AddSingleton<InMemoryEventBusService>();
builder.Services.AddHostedService<WindowsBackgroundService>();

// Add MassTransit
Accelerate.Foundations.Integrations.MassTransit.Startup.ConfigureServices(builder.Services, builder.Configuration);

var host = builder.Build();
host.Run();

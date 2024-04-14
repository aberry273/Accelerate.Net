using Accelerate.Foundations.EventPipelines.Models;
using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using MassTransit;
using MassTransit.DependencyInjection;
using MassTransit.Transports;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Accelerate.Foundations.EventPipelines.Consumers
{

    public class DataCreateCompleteConsumer<T> : IConsumer<CreateCompleteDataContract<T>>
    {
        readonly ILogger<DataCreateCompleteConsumer<T>> _logger;
        IDataCreateCompletedEventPipeline<T> _pipeline;
        IHubContext<BaseHub<T>, IBaseHubClient<WebsocketMessage<T>>> _messageHub;
        public DataCreateCompleteConsumer(
            IHubContext<BaseHub<T>, IBaseHubClient<WebsocketMessage<T>>> messageHub,
            IDataCreateCompletedEventPipeline<T> pipeline,
            ILogger<DataCreateCompleteConsumer<T>> logger)
        {
            _pipeline = pipeline;
            _logger = logger;
            _messageHub = messageHub;
        }

        public async Task Consume(ConsumeContext<CreateCompleteDataContract<T>> context)
        {
            try
            {
                // RUn end comple
                Foundations.Common.Services.StaticLoggingService.Log($"DataCreateCompleteConsumer [Started]]");
                if (_pipeline == null) return;
                // Run synchronous pipeline first
                _pipeline.Run(context.Message.Data);
                //Run async pipeline second
                await _pipeline.RunAsync(context.Message.Data);
            }
            catch (Exception ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
            }
            finally
            {
                Foundations.Common.Services.StaticLoggingService.Log($"DataCreateCompleteConsumer [Complete]]");
            }
        }
    }
}

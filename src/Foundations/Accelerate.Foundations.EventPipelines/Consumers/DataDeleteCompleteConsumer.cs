using Accelerate.Foundations.EventPipelines.Models;
using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using MassTransit;
using MassTransit.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Accelerate.Features.Content.Consumers
{

    public class DataDeleteCompleteConsumer<T> : IConsumer<DeleteCompleteDataContract<T>>
    {
        IDataDeleteCompletedEventPipeline<T> _pipeline;
        readonly ILogger<DataDeleteCompleteConsumer<T>> _logger;
        IHubContext<BaseHub<T>, IBaseHubClient<WebsocketMessage<T>>> _messageHub;
        public DataDeleteCompleteConsumer(
            IDataDeleteCompletedEventPipeline<T> pipeline,
            IHubContext<BaseHub<T>, IBaseHubClient<WebsocketMessage<T>>> messageHub,
            ILogger<DataDeleteCompleteConsumer<T>> logger)
        {
            _logger = logger;
            _messageHub = messageHub;
            _pipeline = pipeline;
        }

        public async Task Consume(ConsumeContext<DeleteCompleteDataContract<T>> context)
        {
            try
            {
                // RUn end comple
                Foundations.Common.Services.StaticLoggingService.Log($"DataDeleteCompleteConsumer [Started]]");
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
                Foundations.Common.Services.StaticLoggingService.Log($"DataDeleteCompleteConsumer [Complete]]");
            }
        }
    }
}

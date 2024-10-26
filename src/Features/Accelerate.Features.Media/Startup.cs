
using Accelerate.Features.Content.Hubs;
using Accelerate.Features.Media.Pipelines.Channels;
using Accelerate.Features.Media.Services;
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Media.EventBus;
using Accelerate.Foundations.Media.Models.Data;
using Accelerate.Foundations.Media.Models.Entities;
using Accelerate.Foundations.Websockets.Hubs;

namespace Accelerate.Features.Media
{
    public static class Startup
    {
        public static void ConfigureApp(WebApplication app)
        {
            app.MapHub<BaseHub<MediaBlobDocument>>($"/{Constants.Websockets.MediaBlobsHubName}");

        } 
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IMediaViewService, MediaViewService>();
            services.AddTransient<BaseHub<MediaBlobDocument>, MediaBlobHub>();
            Foundations.EventPipelines.Startup.ConfigurePipelineServices<MediaBlobEntity, MediaBlobCreatedPipeline, MediaBlobUpdatedPipeline, MediaBlobDeletedPipeline>(services);
            Foundations.EventPipelines.Startup.ConfigureEmptyCompletedPipelineServices<MediaBlobEntity>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<MediaBlobEntity, IMediaBlobEventBus>(services);
        }
    }
}

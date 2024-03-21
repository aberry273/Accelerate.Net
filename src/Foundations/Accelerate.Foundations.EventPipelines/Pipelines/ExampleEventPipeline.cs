using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;

namespace Accelerate.Foundations.EventPipelines.Pipelines
{
    public class ContentPostCreatedPipeline : DataEventPipeline<string>
    {
        public ContentPostCreatedPipeline()
        {
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<string>>()
            {
                AsyncExample
            };
            _processors = new List<PipelineProcessor<string>>()
            {
                AmendName,
                Log
            };
        }
        public Task AsyncExample(IPipelineArgs<string> args)
        {
            // Example to wrap sync task in async processor
            return Task.Run(() =>
            {
                var entity = args.Value;
                if (entity == null) { return; }
            });

        }
        // SYNC PROCESSORS
        public void Log(IPipelineArgs<string> args)
        {
            var data = args.Value;
            if (data == null) { return; }
            StaticLoggingService.Log($"New pipeline created: {data}");
        }
        public void AmendName(IPipelineArgs<string> args)
        {
            var data = args.Value;
            if (data == null) { return; }
            data += "This is an amendment";
        }
    }
}

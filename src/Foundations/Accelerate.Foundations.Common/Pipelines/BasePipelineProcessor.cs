using Accelerate.Foundations.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Pipelines
{

    public delegate Task AsyncPipelineProcessor<T>(IPipelineArgs<T> arg);

    public delegate void PipelineProcessor<T>(IPipelineArgs<T> arg);

    public abstract class BasePipelineProcessor<T> : IPipelineProcessor<T>
    {
        protected IEnumerable<AsyncPipelineProcessor<T>> _asyncProcessors { get; set; } = new List<AsyncPipelineProcessor<T>>();
        protected IEnumerable<PipelineProcessor<T>> _processors { get; set; } = new List<PipelineProcessor<T>>();
        public BasePipelineProcessor() { }
        BasePipelineProcessor(IEnumerable<PipelineProcessor<T>> processors)
        {
            _processors = processors;
        }
        BasePipelineProcessor(IEnumerable<AsyncPipelineProcessor<T>> processors)
        {
            _asyncProcessors = processors;
        }
        // Synchronous process pipeline
        public void Process(IPipelineArgs<T> args)
        {
            Process(_processors, args);
        }
        public void Process<T>(IEnumerable<PipelineProcessor<T>> processors, IPipelineArgs<T> args)
        {
            StaticLoggingService.Log($"PipelineProcess Started");
            foreach (PipelineProcessor<T> p in processors)
            {
                try
                {
                    if (p.GetMethodInfo().ReturnType.DeclaringType == typeof(T)) { }
                    p(args);
                }
                catch (Exception)
                {
                    Console.WriteLine("Error caught");
                }
            }
            StaticLoggingService.Log($"PipelineProcess Finished");
        }
        // Async alternative
        public Task ProcessAsync(IPipelineArgs<T> args)
        {
            return ProcessAsync(_asyncProcessors, args);
        }
        public async Task<(int Index, bool IsDone)> DoAsyncTask(int i)
        {
            Console.WriteLine("working..{0}", i);
            await Task.Delay(1000);
            return (i, true);
        }
        public async Task ProcessAsync<T>(IEnumerable<AsyncPipelineProcessor<T>> processors, IPipelineArgs<T> args)
        {
            StaticLoggingService.Log($"Async PipelineProcess Started");
            await Task.WhenAll(processors.Select(p =>
            {
                try
                {
                    // Create a unique instance per pipeline
                    var arg = new PipelineArgs<T>() { Value = args.Value };
                    return p(args);
                }
                catch(Exception e)
                {
                    Services.StaticLoggingService.LogError(e);
                    return Task.FromException(e);
                }
            }));
            StaticLoggingService.Log($"Async PipelineProcess Finished");
        }
        public void Run(T obj)
        {
            var args = new PipelineArgs<T>() { Value = obj };

            this.Process(args);
        }
        public async Task RunAsync(T obj)
        {
            var args = new PipelineArgs<T>() { Value = obj };

            await this.ProcessAsync(args);
        }
    }
}

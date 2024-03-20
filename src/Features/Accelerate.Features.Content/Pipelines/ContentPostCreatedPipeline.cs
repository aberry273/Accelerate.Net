using Accelerate.Features.Content.Models.Data;
using Accelerate.Features.Content.Pipelines;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Features.Content.Services
{
    public class ContentPostCreatedPipeline : BasePipelineProcessor<ContentPostEntity>, IContentPostCreatedPipeline
    {
        IContentPostElasticService _contentElasticService;
        public ContentPostCreatedPipeline(
            IContentPostElasticService contentElasticService)
        {
            _contentElasticService = contentElasticService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostEntity>>()
            {
                AddToIndex,
                SyncExample
            };
            _processors = new List<PipelineProcessor<ContentPostEntity>>()
            {
                AmendName,
                Log
            };
        }

        // ASYNC PROCESSORS
        public async Task AddToIndex(IPipelineArgs<ContentPostEntity> args)
        {
            var userId = args.Value.UserId.GetValueOrDefault().ToString();
            //var user = await _userManager.FindByIdAsync(userId);
            var indexModel = new ContentPost()
            {
                Content = args.Value.Content,
                User = args.Value.UserId.ToString() ?? "Anonymous"
            };
            var indexResponse = await _contentElasticService.Index(indexModel);
        }
        public Task SyncExample(IPipelineArgs<ContentPostEntity> args)
        {
            // Example to wrap sync task in async processor
            return Task.Run(() =>
            {
                var entity = args.Value;
                if (entity == null) { return; }
            });

        }
        // SYNC PROCESSORS
        public void Log(IPipelineArgs<ContentPostEntity> args)
        {
            var entity = args.Value;
            if (entity == null) { return; }
            StaticLoggingService.Log($"New contact created: {entity.Id}");
        }
        public void AmendName(IPipelineArgs<ContentPostEntity> args)
        {
            var entity = args.Value;
            if (entity == null) { return; }
            entity.Content += "This is an amendment";
        }
    }
}

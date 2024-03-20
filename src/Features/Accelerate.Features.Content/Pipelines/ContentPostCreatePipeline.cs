using Accelerate.Features.Content.Models.Data;
using Accelerate.Features.Content.Pipelines;
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
    public class ContentPostCreatePipeline : BasePipelineProcessor<ContentPostEntity>, IContentPostCreatePipeline
    {
        IContentPostElasticService _contentElasticService;
        public ContentPostCreatePipeline(IContentPostElasticService contentElasticService)
        {
            _contentElasticService = contentElasticService;
            // To update as reflection / auto load based on inheritance classes in library
            _asyncProcessors = new List<AsyncPipelineProcessor<ContentPostEntity>>()
            {
                AddToIndex,
                AmendName,
                Log
            };
        }

        public async Task AddToIndex(IPipelineArgs<ContentPostEntity> args)
        {
            //var userId = obj.UserId.GetValueOrDefault().ToString();
            //var user = await _userManager.FindByIdAsync(userId);
            var indexModel = new ContentPost()
            {
                Content = args.Value.Content,
                User = args.Value.UserId.ToString()
            };
            var indexResponse = await _contentElasticService.Index(indexModel);
        }
        // PROCESSORS
        public Task Log(IPipelineArgs<ContentPostEntity> args)
        {
            return Task.Run(() =>
            {
                var entity = args.Value;
                if (entity == null) { return; }
                StaticLoggingService.Log(entity.Content);
            });

        }
        public Task AmendName(IPipelineArgs<ContentPostEntity> args)
        {
            return Task.Run(() =>
            {
                var entity = args.Value;
                if (entity == null) { return; }
                entity.Content += "This is an amendment";
            });
        }
    }
}

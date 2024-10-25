using Accelerate.Foundations.Common.Pipelines;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Websockets.Hubs;
using Accelerate.Foundations.Websockets.Models;
using Microsoft.AspNetCore.SignalR;

namespace Accelerate.Features.Content.Pipelines.Parents
{
    public static class ContentPostParentUtilities
    {
        public static int GetTotalReplies(IEntityService<ContentPostParentEntity> entityService, IPipelineArgs<ContentPostParentEntity> args)
        {
            return entityService
               .Find(x => x.ParentId == args.Value.ParentId)
               .GroupBy(g => 1)
               .Count();
        }
    }
}

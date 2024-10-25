using Accelerate.Foundations.Content.EventBus;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Accelerate.Foundations.Integrations.Elastic.Services;
using MassTransit.DependencyInjection;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.Integrations.Elastic.Models;
using Accelerate.Foundations.EventPipelines.EventBus;

namespace Accelerate.Features.Content.Controllers.Api
{
    [ApiController]
    public class BaseContentApiController<E, D, B> : BaseApiController<E> 
        where E : IBaseEntity
        where D : EntityDocument
        where B : IDataBus<E>
    {
        readonly Bind<B, IPublishEndpoint> _publishEndpoint;
        public BaseContentApiController(
            IEntityService<E> service,
            Bind<B, IPublishEndpoint> publishEndpoint) : base(service)
        {
            _publishEndpoint = publishEndpoint;
        }


        protected override async Task PostCreateSteps(E obj)
        {
            await _publishEndpoint.Value.Publish(new CreateDataContract<E>() { Data = obj });
        }
        protected override void UpdateValues(E from, dynamic to)
        {
        }
        protected override async Task PostUpdateSteps(E obj)
        {
            await _publishEndpoint.Value.Publish(new UpdateDataContract<E>() { Data = obj });
        }
        protected override async Task PostDeleteSteps(E obj)
        {
            await _publishEndpoint.Value.Publish(new DeleteDataContract<E>() { Data = obj });
        }
    }
}

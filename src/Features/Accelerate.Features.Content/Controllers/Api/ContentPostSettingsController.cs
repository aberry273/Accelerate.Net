using Accelerate.Foundations.Content.EventBus;
using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Accelerate.Foundations.Integrations.Contracts;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Websockets.Hubs;
using MassTransit;
using MassTransit.DependencyInjection;
using MassTransit.Transports;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using static Accelerate.Foundations.Database.Constants.Exceptions;

namespace Accelerate.Features.Content.Controllers.Api
{
    [Route("api/[controller]")]
    public class ContentPostSettingsController : BaseContentApiController<ContentPostSettingsEntity, ContentPostSettingsDocument, IContentSettingsBus>
    { 
        public ContentPostSettingsController(IEntityService<ContentPostSettingsEntity> service,
            Bind<IContentSettingsBus, IPublishEndpoint> publishEndpoint) : base(service, publishEndpoint)
        {
        }

        protected override void UpdateValues(ContentPostSettingsEntity from, dynamic to)
        {
        }
    }
}
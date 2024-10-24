using Accelerate.Foundations.Content.EventBus;
using Accelerate.Foundations.Account.Models.Entities;
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
using Accelerate.Foundations.Operations.Models.Entities;

namespace Accelerate.Features.Admin.Controllers.Api
{ 
    public class AdminActionController : BaseApiServiceController<OperationsActionEntity>
    { 
        UserManager<AccountUser> _userManager;
        IMetaContentService _contentService;  
        IEntityService<ContentPostEntity> _postService;
        public AdminActionController(
            IMetaContentService contentService,
            IEntityService<OperationsActionEntity> service,
            IEntityService<ContentPostEntity> postService,  
            UserManager<AccountUser> userManager) : base(service)
        { 
            _userManager = userManager;
            _contentService = contentService; 
            _postService = postService;
        }
        protected override async Task PostCreateSteps(OperationsActionEntity obj)
        {
        }
        protected override async Task PostUpdateSteps(OperationsActionEntity obj)
        {
        }
        protected override async Task PostDeleteSteps(OperationsActionEntity obj)
        {
        }
        protected override void UpdateValues(OperationsActionEntity from, dynamic to)
        {
            from.Name = to.Name;
            from.Data = to.Data;
            from.Action = to.Action;
            from.Settings = to.Settings;
            from.State = to.State;
        } 
    }
}
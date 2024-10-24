
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.Data;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Accelerate.Foundations.Integrations.AzureStorage.Services;
using Azure.Storage.Blobs;
using Elastic.Clients.Elasticsearch.Core.Search;
using MassTransit.DependencyInjection;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.StaticFiles;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.IO;
using System.Text;
using static Accelerate.Foundations.Database.Constants.Exceptions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Hosting.Server;
using Accelerate.Foundations.Account.EventBus;

namespace Accelerate.Projects.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AccountUserApiController : ControllerBase
    {
        readonly Bind<IAccountBus, IPublishEndpoint> _publishEndpoint;
        SignInManager<AccountUser> _signInManager;
        UserManager<AccountUser> _userManager;
        IEntityService<AccountProfile> _profileService;
        IMetaContentService _contentService;
        public AccountUserApiController(
            IMetaContentService contentService,
            UserManager<AccountUser> userManager,
            SignInManager<AccountUser> signInManager,
            IEntityService<AccountProfile> profileService,
            Bind<IAccountBus, IPublishEndpoint> publishEndpoint)
        {
            _userManager = userManager;
            _contentService = contentService;
            _signInManager = signInManager;
            _publishEndpoint = publishEndpoint;
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var userObj = new
            {
                id = user.Id,
                Username = user.UserName,
            };
            return Ok(user);
        }

        protected async Task PostDeleteSteps(AccountUser obj)
        {
            await _publishEndpoint.Value.Publish(new DeleteDataContract<AccountUser>()
            {
                Data = obj,
                UserId = obj.Id
            });
        }
        protected async Task PostUpdateSteps(AccountUser obj)
        {
            await _publishEndpoint.Value.Publish(new UpdateDataContract<AccountUser>()
            {
                Data = obj,
                UserId = obj.Id
            });
        }
    }
}
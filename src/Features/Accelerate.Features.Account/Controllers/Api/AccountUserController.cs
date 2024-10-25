using Accelerate.Features.Account.Models.Views;
using Accelerate.Features.Content.EventBus;
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
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Features.Account.Models.Data;

namespace Accelerate.Features.Content.Controllers.Api
{

    [Route("api/[controller]")]
    [ApiController]
    public class AccountUserController : ControllerBase
    {
        readonly Bind<IAccountBus, IPublishEndpoint> _publishEndpoint;
        SignInManager<AccountUser> _signInManager;
        UserManager<AccountUser> _userManager;
        IEntityService<AccountProfile> _profileService;
        IMetaContentService _contentService;
        public AccountUserController(
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


        [HttpPut("{userId}")]
        public async Task<IStatusCodeActionResult> Update([FromRoute] Guid userId, [FromBody] UpdateUserForm model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                {
                    return NotFound();
                }
                //Users cannot change the username if they have already changed it
                if(user.UserName != user.Email)
                {
                    return BadRequest("Username cannot be updated");
                }
                var existingUser = await _userManager.FindByNameAsync(model.Username);
                if(existingUser != null)
                {
                    return BadRequest("That username is taken");
                }

                user.UserName = model.Username;

                await _userManager.UpdateAsync(user);

                await PostUpdateSteps(user);

                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        [HttpPost("delete")]
        public async Task<IStatusCodeActionResult> Delete([FromBody] DeactivateForm model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.UserId.ToString());

                if (user == null)
                {
                    return NotFound();
                }
                if(user.Status != AccountUserStatus.Deactivated)
                {
                    return Problem("User not deactivated");
                }
                var deactivatedUsername = user.Id.ToString();
                user.Status = AccountUserStatus.Deleted;
                user.Email = deactivatedUsername + "@deleted.parot.app";
                user.UserName = deactivatedUsername;
                user.UpdatedOn = DateTime.Now;
                user.AccountProfileId = Guid.Empty;
                await _userManager.UpdateAsync(user);
                var logins = await _userManager.GetLoginsAsync(user);
                foreach(var login in logins)
                {
                    await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
                }
                var profile = _profileService.Get(user.AccountProfileId);
                if (profile != null)
                {
                    await _profileService.Delete(profile);
                }

                await PostDeleteSteps(user);
                await _signInManager.SignOutAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }
        [HttpPost("deactivate")]
        public async Task<IStatusCodeActionResult> Deactivate([FromBody] DeactivateForm model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.UserId.ToString());

                if (user == null)
                {
                    return NotFound();
                }
                user.Status = AccountUserStatus.Deactivated;
                await _userManager.UpdateAsync(user);
                await PostUpdateSteps(user);
                
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }
        [HttpPost("reactivate")]
        public async Task<IStatusCodeActionResult> Reactivate([FromBody] DeactivateForm model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.UserId.ToString());

                if (user == null)
                {
                    return NotFound();
                }
                user.Status = AccountUserStatus.Active;
                await _userManager.UpdateAsync(user);
                await PostUpdateSteps(user);

                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
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
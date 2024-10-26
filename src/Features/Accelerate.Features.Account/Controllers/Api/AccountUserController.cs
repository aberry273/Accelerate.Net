using Accelerate.Features.Account.Models.Views;
using Accelerate.Foundations.Users.Models.Entities;
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
using Accelerate.Foundations.Users.EventBus;

namespace Accelerate.Features.Content.Controllers.Api
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsersUserController : ControllerBase
    {
        readonly Bind<IUsersBus, IPublishEndpoint> _publishEndpoint;
        SignInManager<UsersUser> _signInManager;
        UserManager<UsersUser> _userManager;
        IEntityService<UsersProfile> _profileService;
        IMetaContentService _contentService;
        public UsersUserController(
            IMetaContentService contentService,
            UserManager<UsersUser> userManager,
            SignInManager<UsersUser> signInManager,
            IEntityService<UsersProfile> profileService,
            Bind<IUsersBus, IPublishEndpoint> publishEndpoint)
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
                if(user.Status != UsersUserStatus.Deactivated)
                {
                    return Problem("User not deactivated");
                }
                var deactivatedUsername = user.Id.ToString();
                user.Status = UsersUserStatus.Deleted;
                user.Email = deactivatedUsername + "@deleted.parot.app";
                user.UserName = deactivatedUsername;
                user.UpdatedOn = DateTime.Now;
                user.UsersProfileId = Guid.Empty;
                await _userManager.UpdateAsync(user);
                var logins = await _userManager.GetLoginsAsync(user);
                foreach(var login in logins)
                {
                    await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
                }
                var profile = _profileService.Get(user.UsersProfileId.GetValueOrDefault());
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
                user.Status = UsersUserStatus.Deactivated;
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
                user.Status = UsersUserStatus.Active;
                await _userManager.UpdateAsync(user);
                await PostUpdateSteps(user);

                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }
        protected async Task PostDeleteSteps(UsersUser obj)
        {
            await _publishEndpoint.Value.Publish(new DeleteDataContract<UsersUser>()
            {
                Data = obj,
                UserId = obj.Id
            });
        }
        protected async Task PostUpdateSteps(UsersUser obj)
        {
            await _publishEndpoint.Value.Publish(new UpdateDataContract<UsersUser>()
            {
                Data = obj,
                UserId = obj.Id
            });
        }
    }
}
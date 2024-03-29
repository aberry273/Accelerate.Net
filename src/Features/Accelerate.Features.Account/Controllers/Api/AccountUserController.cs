using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Database.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Drawing;
using static Accelerate.Foundations.Database.Constants.Exceptions;

namespace Accelerate.Features.Content.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountUserController : ControllerBase
    { 
        UserManager<AccountUser> _userManager;
        IMetaContentService _contentService;  
        public AccountUserController(
            IMetaContentService contentService,
            IEntityService<AccountUser> service,
            UserManager<AccountUser> userManager)
        {
            _userManager = userManager;
            _contentService = contentService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] RequestQuery<AccountUser> query)
        {
            var user = await _userManager.GetUserAsync(this.User);
            var userObj = new
            {
                id = user.Id,
                Username = user.UserName,
            };
            return Ok(user);
        }
    }
}
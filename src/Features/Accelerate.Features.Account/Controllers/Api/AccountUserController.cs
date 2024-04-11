using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Integrations.AzureStorage.Services;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.StaticFiles;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.IO;
using System.Text;
using static Accelerate.Foundations.Database.Constants.Exceptions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Accelerate.Features.Content.Controllers.Api
{ 

    [Route("api/[controller]")]
    [ApiController]
    public class AccountUserController : ControllerBase
    { 
        UserManager<AccountUser> _userManager;
        IMetaContentService _contentService;
        public AccountUserController(
            IMetaContentService contentService,
            UserManager<AccountUser> userManager)
        {
            _userManager = userManager;
            _contentService = contentService;
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
       
    }
}
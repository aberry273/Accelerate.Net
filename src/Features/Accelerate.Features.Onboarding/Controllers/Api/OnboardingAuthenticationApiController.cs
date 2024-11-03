
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.Data;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.Models.Contracts;
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
using Accelerate.Foundations.Users.EventBus;
using Accelerate.Foundations.Communication.Services;
using Accelerate.Features.Onboarding.Models.Data;

namespace Accelerate.Features.Content.Controllers.Api
{

    [Route($"{Foundations.Common.Constants.ApiPaths.VersionPath}/onboarding/authentication")]
    [ApiController]
    public class OnboardingAuthenticationApiController : ControllerBase
    {
        SignInManager<UsersUser> _signInManager;
        UserManager<UsersUser> _userManager;
        IMessageService _messageService;
        public OnboardingAuthenticationApiController(
            IMessageService messageService,
            UserManager<UsersUser> userManager,
            SignInManager<UsersUser> signInManager)
        {
            _userManager = userManager;
            _messageService = messageService;
            _signInManager = signInManager;
        }

        [HttpPost("resendcode")]
        public async Task<IStatusCodeActionResult> ResendCode([FromBody] OnboardingAuthenticationApiResendCodeRequest model)
        {
            try
            {
                var response = new OperationResponse<bool>()
                {
                    Success = true,
                };
                // Find user
                var user = await _userManager.FindByIdAsync(model.UserId.ToString());

                if (user == null)
                {
                    response.Message = "Not found";
                    return NotFound(response);
                }
             
                // Send Email OTP
                var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.Provider);
                var message = "Your security code is: " + code;
                if (model.Provider == "Email")
                {
                    await _messageService.SendEmailAsync(user.Email, "Security Code", message);
                }
                else if (model.Provider == "Phone")
                {
                    //await _messageService.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
                }
                response.Success = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }
    }
}
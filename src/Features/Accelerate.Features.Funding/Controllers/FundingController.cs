using Accelerate.Features.Content.Controllers;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Funding.Models.Entities;
using Accelerate.Foundations.Users.Attributes;
using Accelerate.Foundations.Users.Services;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Features.Funding.Controllers
{
    public class FundingController : Controller
    {
        IUsersUserService _userService;
        public FundingController(IUsersUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [RedirectUnauthenticatedRoute(url = Foundations.Users.Constants.Paths.UnauthenticatedRedirectUrl)]
        public async Task<IActionResult> Index()
        {
            var user = await _userService.FindByClaimAsync(this.User);

            if (user == null) return RedirectToAction("NotFound", "Funding");

            return RedirectToAction(nameof(BankAccountController.Listing), Foundations.Common.Helpers.ControllerHelper.NameOf<BankAccountController>());
        }
    }
}

using Accelerate.Features.Content.Controllers;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Users.Attributes;
using Accelerate.Foundations.Users.Services;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Features.Rates.Controllers
{
    public class RatesController : Controller
    {
        IUsersUserService _userService;
        public RatesController(IUsersUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [RedirectUnauthenticatedRoute(url = Foundations.Users.Constants.Paths.UnauthenticatedRedirectUrl)]
        public async Task<IActionResult> Index()
        {
            var user = await _userService.FindByClaimAsync(this.User);

            if (user == null) return RedirectToAction("NotFound", "Rates");

            return RedirectToAction(nameof(QuoteController.Listing), Foundations.Common.Helpers.ControllerHelper.NameOf<QuoteController>());
        }
    }
}

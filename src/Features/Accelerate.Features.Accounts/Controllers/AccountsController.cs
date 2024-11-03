using Accelerate.Features.Content.Controllers;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Users.Attributes;
using Accelerate.Foundations.Users.Services;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Features.Accounts.Controllers
{
    public class AccountsController : Controller
    {
        IUsersUserService _userService;
        public AccountsController(IUsersUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [RedirectUnauthenticatedRoute(url = Foundations.Users.Constants.Paths.UnauthenticatedRedirectUrl)]
        public async Task<IActionResult> Index()
        {
            var user = await _userService.FindByClaimAsync(this.User);
            if (user == null) return RedirectToAction("NotFound", "Accounts");

            if (await _userService.UserInRole(user, Foundations.Portal.Constants.Roles.AccountIndividual))
            {
                return RedirectToAction(nameof(IndividualController.Index), Foundations.Common.Helpers.ControllerHelper.NameOf<IndividualController>());
            }
            else if (await _userService.UserInRole(user, Foundations.Portal.Constants.Roles.AccountBusiness))
            {
                return RedirectToAction(nameof(BusinessController.Index), Foundations.Common.Helpers.ControllerHelper.NameOf<BusinessController>());
            }
            return RedirectToAction("NotFound", "Accounts");
        }
    }
}

using Accelerate.Features.Content.Controllers;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Transactions.Models.Entities;
using Accelerate.Foundations.Users.Attributes;
using Accelerate.Foundations.Users.Services;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Features.Transactions.Controllers
{
    public class TransactionsController : Controller
    {
        IUsersUserService _userService;
        public TransactionsController(IUsersUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [RedirectUnauthenticatedRoute(url = Foundations.Users.Constants.Paths.UnauthenticatedRedirectUrl)]
        public async Task<IActionResult> Index()
        {
            var user = await _userService.FindByClaimAsync(this.User);

            if (user == null) return RedirectToAction("NotFound", "Transactions");

            return RedirectToAction(nameof(TransactionController.Listing), Foundations.Common.Helpers.ControllerHelper.NameOf<TransactionController>());
        }
    }
}

using Accelerate.Foundations.Account.Attributes;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Account.Services;
namespace Accelerate.Projects.Api.Controllers
{
    //[Authorize]
    public class HomeController : BaseController
    {
        UserManager<AccountUser> _userManager;
        IMetaContentService _contentService;
        IAccountUserService _accountUserService;
        public HomeController(
            IAccountUserService accountUserService,
            UserManager<AccountUser> userManager,
            IMetaContentService contentService)
            : base(contentService)
        {
            _contentService = contentService;
            _userManager = userManager;
            _accountUserService = accountUserService;
        }

        public IActionResult Index()
        {
            return Redirect("/Swagger");
        }

    }
}

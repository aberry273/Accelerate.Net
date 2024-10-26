using Accelerate.Foundations.Users.Attributes;
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Features.Content.Controllers;
using Accelerate.Foundations.Users.Services;
namespace Accelerate.Projects.App.Controllers
{
    //[Authorize]
    public class HomeController : BaseController
    {
        UserManager<UsersUser> _userManager;
        IMetaContentService _contentService;
        IUsersUserService _UsersUserService;
        public HomeController(
            IUsersUserService UsersUserService,
            UserManager<UsersUser> userManager,
            IMetaContentService contentService)
            : base(contentService)
        {
            _contentService = contentService;
            _userManager = userManager;
            _UsersUserService = UsersUserService;
        }
        private BasePage CreateBaseContent(UsersUser user)
        { 
            var profile = Accelerate.Foundations.Users.Helpers.UsersHelpers.CreateUserProfile(user);
            return _contentService.CreatePageBaseContent(profile);
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(FeedsController.Index), Foundations.Common.Helpers.ControllerHelper.NameOf<FeedsController>());
        }

    }
}

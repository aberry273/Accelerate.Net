using Accelerate.Features.Account.Attributes;
using Accelerate.Features.Account.Models.Entities;
using Accelerate.Features.Account.Models.Views;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Projects.App.Controllers
{
    //[Authorize]
    public class HomeController : BaseController
    {
        ISharedContentService _contentService;
        public HomeController(
            ISharedContentService contentService)
            : base(contentService)
        {
            _contentService = contentService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Feed");
        }

        public IActionResult Feed()
        {
            var model = _contentService.CreatePageBaseContent();
            return View(model);
        }
    }
}

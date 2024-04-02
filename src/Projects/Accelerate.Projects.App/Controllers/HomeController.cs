﻿using Accelerate.Foundations.Account.Attributes;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Features.Account.Models.Views;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.Views;

namespace Accelerate.Projects.App.Controllers
{
    //[Authorize]
    public class HomeController : BaseController
    {
        UserManager<AccountUser> _userManager;
        IMetaContentService _contentService;
        public HomeController(
            UserManager<AccountUser> userManager,
            IMetaContentService contentService)
            : base(contentService)
        {
            _contentService = contentService;
            _userManager = userManager;
        }
        private BasePage CreateBaseContent(AccountUser user)
        {
            var profile = user != null ? new UserProfile()
            {
                Username = user.UserName,
            } : null;
            return _contentService.CreatePageBaseContent(profile);
        }

        public IActionResult Index()
        {
            return RedirectToAction("Feed", "Content");
        }

    }
}
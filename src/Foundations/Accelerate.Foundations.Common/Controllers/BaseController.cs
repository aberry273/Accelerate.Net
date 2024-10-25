using Accelerate.Foundations.Common.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Security.Principal;
using System.Security.Claims;

namespace Accelerate.Foundations.Common.Controllers
{
    public abstract class BaseController : Controller
    {
        protected IMetaContentService _sharedContentService;
        protected ClaimsPrincipal _identity => this.User;
        public BaseController(IMetaContentService sharedContentService)
        {
            _sharedContentService = sharedContentService;
        }
        /*
        public IActionResult Index()
        {
            var model = _sharedContentService.CreatePageBaseContent();
            return View(model);
        }
        */
    }
}

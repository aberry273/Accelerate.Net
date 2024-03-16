using Accelerate.Foundations.Common.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Controllers
{
    public abstract class BaseController : Controller
    {
        protected ISharedContentService _sharedContentService;
        public BaseController(ISharedContentService sharedContentService)
        {
            _sharedContentService = sharedContentService;
        }
        public IActionResult Index()
        {
            var model = _sharedContentService.CreatePageBaseContent();
            return View(model);
        }
    }
}

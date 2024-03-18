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
        protected IMetaContentService _sharedContentService;
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

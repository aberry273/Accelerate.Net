using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Helpers
{
    public static class ControllerHelper
    {
        public static string NameOf<T>() where T : Controller
        {
            return typeof(T).Name.Replace(nameof(Controller), string.Empty);
        }
    }
}

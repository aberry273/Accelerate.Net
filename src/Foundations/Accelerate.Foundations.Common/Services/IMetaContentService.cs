using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Services
{
    public interface IMetaContentService
    {
        string GetActionUrl(string action, string controller, object values = null, string protocol = null);
        BasePage CreatePageBaseContent(UserProfile? profile = null);
    }
}

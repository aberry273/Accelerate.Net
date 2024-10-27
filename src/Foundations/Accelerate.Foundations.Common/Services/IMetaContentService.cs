using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Services
{
    public interface IMetaContentService
    {
        string GetCurrentUrl();
        string GetActionUrl(string action, string controller, object values = null, string protocol = null);
        FormField FormField(string name, FormFieldComponents component, string cssClass, string placeholder, object? value, bool disabled = false, bool hidden = false, int? min = null, int? max = null, bool? multiple = false, string? label = null);
        FormField FormFieldItems(string name, FormFieldComponents component, List<dynamic> items, string cssClass, string placeholder, object? value, bool disabled = false, bool hidden = false, int? min = null, int? max = null, bool? multiple = false, string? label = null);
        BasePage CreatePageBaseContent(UserProfile? profile = null);
    }
}

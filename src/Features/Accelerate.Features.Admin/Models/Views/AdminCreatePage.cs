﻿
using Accelerate.Features.Admin.Models.Views;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Content.Models.Data;

namespace Accelerate.Features.Admin.Models.Views
{
    public class AdminCreatePage : AdminBasePage
    {
        public string RedirectRoute { get; set; }
        public AjaxForm Form { get; set; }
        public AdminCreatePage(AdminBasePage model) : base(model)
        {
        }
    }
}
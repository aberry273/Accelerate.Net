
using Accelerate.Features.Admin.Models.Views;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.UI.Components.Table;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Content.Models.Data;

namespace Accelerate.Features.Admin.Models.Views
{
    public class AdminIndexPage<T> : AdminBasePage
    {
        public string RedirectRoute { get; set; }
        public AjaxForm Form { get; set; }
        public AclTable<string> Table { get; set; }
        public T Item { get; set; }
        public AdminIndexPage(AdminBasePage model) : base(model)
        {
        }
    }
}

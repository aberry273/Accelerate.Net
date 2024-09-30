using Accelerate.Features.Content.Models.Data;
using Accelerate.Features.Content.Models.UI;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Content.Models.Data;

namespace Accelerate.Features.Content.Models.Views
{
    public class ContentPage : ContentBasePage
    { 
        public ContentSubmitForm FormCreatePost { get; set; }
        public ModalForm ModalCreate { get; set; }
        public ModalForm ModalEdit { get; set; }
        public ModalForm ModalCreateLabel { get; set; }
        public ContentPage(ContentBasePage model) : base(model)
        {
        }
    }
}

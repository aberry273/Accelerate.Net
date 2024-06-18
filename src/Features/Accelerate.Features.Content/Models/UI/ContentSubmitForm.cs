using Accelerate.Foundations.Common.Models.UI.Components;

namespace Accelerate.Features.Content.Models.UI
{
    public class ContentSubmitForm : AjaxForm
    {
        public Guid UserId { get; set; }
        public string SearchUsersUrl { get; set; }
        public string FetchMetadataUrl { get; set; }
    }
}

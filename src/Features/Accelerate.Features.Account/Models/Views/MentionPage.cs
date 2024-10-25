using Accelerate.Foundations.Common.Models.UI.Components.Table;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Content.Models.Entities;

namespace Accelerate.Features.Account.Models.Views
{
    public class MentionPage : ManagePage
    {
        public MentionPage(ManagePage model) : base(model)
        {
            this.UserStatus = model.UserStatus;
            this.ActionUrl = model.ActionUrl;
            this.SearchUrl = model.SearchUrl;
            this.FilterEvent = model.FilterEvent;
            this.DeactivateForm = model.DeactivateForm;
            this.DeleteForm = model.DeleteForm;
            this.ReactivateForm = model.ReactivateForm;
            this.UserForm = model.UserForm;
            this.ProfileImageForm = model.ProfileImageForm;
            this.ProfileForm = model.ProfileForm;
            this.ModalCreateMedia = model.ModalCreateMedia;
            this.UserId = model.UserId;
            this.Tabs = model.Tabs;
            this.Filters = model.Filters;
        }

        public AjaxAclTable<ContentPostActivityEntity> Table { get; set; }
    }
}

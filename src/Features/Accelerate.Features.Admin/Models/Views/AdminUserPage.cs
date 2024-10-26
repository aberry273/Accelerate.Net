using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Models.UI.Components;

namespace Accelerate.Features.Admin.Models.Views
{
    public class AdminUserPage : AdminIndexPage<UsersUser>
    {
        public string LastIndex { get; set; }
        public bool HasProfile { get; set; }
        public ModalForm ModalIndex { get; set; }
        public ModalForm ModalProfile { get; set; }

        public AjaxForm DeactivateForm { get; set; }
        public AjaxForm DeleteForm { get; set; }
        public AjaxForm ReactivateForm { get; set; }
        public AjaxForm UserForm { get; set; }
        public AjaxForm ProfileImageForm { get; set; }
        public AjaxForm ProfileForm { get; set; }

        public AdminUserPage(AdminIndexPage<UsersUser> model) : base(model)
        {
            this.Id = model.Id;
            this.UserId = model.UserId;
            this.Item = model.Item;
            this.RedirectRoute = model.RedirectRoute; 
            this.ActionEvent = model.ActionEvent;
            this.ServiceSettings = model.ServiceSettings; 
        }
    }
}

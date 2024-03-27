using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Content.Models;

namespace Accelerate.Features.Content.Services
{
    public interface IContentViewService
    {
        public AjaxForm CreatePostForm(AccountUser user);
        public AjaxForm CreateReplyForm(AccountUser user, ContentPostEntity post);
        public ModalForm CreateModalEditReplyForm(AccountUser user);
        public AjaxForm CreateFormEditReply(AccountUser user);
        public ModalForm CreateModalDeleteReplyForm(AccountUser user);
        public AjaxForm CreateFormDeleteReply(AccountUser user);
    }
}

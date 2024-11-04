using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Database.Models;

namespace Accelerate.Features.Transactions.Models.Views
{
    public class TransactionsBasePage<T> : TransactionsBasePage where T : IBaseEntity
    {
        public TransactionsBasePage(TransactionsBasePage<T> model) : base(model)
        {
            Id = model.Id;
            Entity = model.Entity;
            ModalDelete = model.ModalDelete;
            PageActions = model.PageActions;
            PageLinks = model.PageLinks;
            ModalEditChannel = model.ModalEditChannel;
            ModalDeleteChannel = model.ModalDeleteChannel;
        }
        public TransactionsBasePage(BasePage model) : base(model)
        {
        }

        public T? Entity { get; set; }
    }
    public class TransactionsBasePage : BasePage
    {
        public TransactionsBasePage(TransactionsBasePage model) : base(model)
        {
            Id = model.Id;
            ModalDelete = model.ModalDelete;
            PageActions = model.PageActions;
            PageLinks = model.PageLinks;
            ModalEditChannel = model.ModalEditChannel;
            ModalDeleteChannel = model.ModalDeleteChannel;
        }
        public TransactionsBasePage(BasePage model) : base(model)
        {
        }

        public string Name { get; set; }
        public Guid Id { get; set; } 
        public ButtonGroup? PageActions { get; set; }
        public List<NavigationGroup>? PageLinks { get; set; }
        public ModalForm? ModalDelete { get; set; }
        public ModalForm? ModalEditChannel { get; set; }
        public ModalForm? ModalDeleteChannel { get; set; }
    }
}

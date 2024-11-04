using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Database.Models;

namespace Accelerate.Features.Rates.Models.Views
{
    public class RatesBasePage<T> : RatesBasePage where T : IBaseEntity
    {
        public RatesBasePage(RatesBasePage<T> model) : base(model)
        {
            Id = model.Id;
            Entity = model.Entity;
            ModalDelete = model.ModalDelete;
            PageActions = model.PageActions;
            PageLinks = model.PageLinks;
            ModalEditChannel = model.ModalEditChannel;
            ModalDeleteChannel = model.ModalDeleteChannel;
        }
        public RatesBasePage(BasePage model) : base(model)
        {
        }

        public T? Entity { get; set; }
    }
    public class RatesBasePage : BasePage
    {
        public RatesBasePage(RatesBasePage model) : base(model)
        {
            Id = model.Id;
            ModalDelete = model.ModalDelete;
            PageActions = model.PageActions;
            PageLinks = model.PageLinks;
            ModalEditChannel = model.ModalEditChannel;
            ModalDeleteChannel = model.ModalDeleteChannel;
        }
        public RatesBasePage(BasePage model) : base(model)
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

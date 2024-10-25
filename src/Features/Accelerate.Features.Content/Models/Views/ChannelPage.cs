﻿using Accelerate.Features.Content.Models.Data;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Content.Models.Data;

namespace Accelerate.Features.Content.Models.Views
{
    public class ChannelPage : BasePage
    {
        public string RouteName { get; set; }
        public Guid UserId { get; set; }
        public ContentChannelDocument Item { get; set; }
        public NavigationGroup ChannelsTabs { get; set; }
        public List<NavigationItem> ChannelDropdown { get; set; }
        public AjaxForm FormCreateReply { get; set; }
        public ModalForm ModalEditChannel { get; set; }
        public ModalForm ModalDeleteChannel { get; set; }
        //public ModalForm ModalEditReply { get; set; }
        //public ModalForm ModalDeleteReply { get; set; }
        public NavigationFilter Filters { get; set; }
        public string FilterEvent { get; set; }
        public string ActionEvent { get; set; }
        public string ActionsApiUrl { get; set; }
        public string PostsApiUrl { get; set; }
        public ChannelPage(BasePage model) : base(model)
        {

        }
    }
}

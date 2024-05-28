﻿using Accelerate.Features.Content.Models.Data;
using Accelerate.Features.Content.Models.UI;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;

namespace Accelerate.Features.Content.Models.Views
{
    public class ThreadPage : BasePage
    {
        public Guid UserId { get; set; }
        public ContentPostDocument Item { get; set; }
        public List<ContentPostDocument> Parents { get; set; } = new List<ContentPostDocument>();
        public List<ContentPostDocument> Replies { get; set; } = new List<ContentPostDocument>();
        public string FilterEvent { get; set; } = "filter:update";
        public string ActionEvent { get; set; } = "action:post";
        public string ActionsApiUrl { get; set; }
        public string PostsApiUrl { get; set; }
        public string ParentPostsApiUrl { get; set; }
        public NavigationItem ParentLink { get; set; }
        public NavigationItem ChannelLink { get; set; }
        public List<NavigationFilter> Filters { get; set; } = new List<NavigationFilter>(); 
        public ContentSubmitForm FormCreateReply { get; set; }
        public ModalForm ModalEditReply { get; set; }
        public ModalForm ModalDeleteReply { get; set; }
        public ThreadPage(BasePage model) : base(model)
        {

        }
    }
}

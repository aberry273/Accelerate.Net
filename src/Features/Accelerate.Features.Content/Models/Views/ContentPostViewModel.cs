﻿using Accelerate.Foundations.Common.Models;

namespace Accelerate.Features.Content.Models.Views
{
    public class ContentPostViewModel : IEntityViewModel
    {
        public string? Content { get; set; }
        public Guid UserId { get; set; }
    }
}

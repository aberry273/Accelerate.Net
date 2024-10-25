﻿using Accelerate.Foundations.Database.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Content.Models.Entities
{
    [Table("ContentPostActions")]
    public class ContentPostActionsEntity : BaseEntity
    {
        [JsonIgnore]
        public virtual ContentPostEntity? ContentPost { get; set; }
        [ForeignKey("ContentPost")]
        public Guid ContentPostId { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        /// <summary>
        /// Post replies set this as the root node
        /// </summary>
        public bool? Agree { get; set; }
        public bool? Disagree { get; set; }
        public bool? Like { get; set; }
        public string? Reaction { get; set; }
    }
}
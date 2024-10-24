using Accelerate.Foundations.Integrations.Elastic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Content.Models.Data
{
    public class ContentEntityDocument : EntityDocument
    {
        public virtual string Name { get; set; }
        public virtual string? Description { get; set; }
        public virtual Guid? UserId { get; set; }
        public virtual string? Category { get; set; }
        public virtual IEnumerable<string>? Tags { get; set; }
    }
}

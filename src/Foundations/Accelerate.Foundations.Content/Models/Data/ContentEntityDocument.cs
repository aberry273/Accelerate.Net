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
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid? UserId { get; set; }
        public string? Category { get; set; }
        public IEnumerable<string>? Tags { get; set; }
    }
}

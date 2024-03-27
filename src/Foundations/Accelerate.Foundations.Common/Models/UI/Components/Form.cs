using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Models.UI.Components
{
    public class Form
    {
        public Form() { }
        public List<FormField> Fields { get; set; }
        public required string Label { get; set; }
    }
}

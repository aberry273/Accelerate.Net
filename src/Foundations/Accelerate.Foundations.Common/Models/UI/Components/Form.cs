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
        public string Method { get; set; } = "POST";
        public string Response { get; set; }
        public List<FormField> Fields { get; set; }
        public string Label { get; set; }
    }
}

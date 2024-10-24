using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Models.UI.Components
{
    public class AclButtonAction
    {
        public string Name { get; set; }
        public string Tooltip { get; set; }
        public string Icon { get; set; }
        public string Event { get; set; }
        public object Value { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Models.UI.Components
{
    public class AjaxForm : Form
    {
        public required string Action { get; set; }
        public bool IsFile { get; set; }
        public string Event { get; set; }
        public string ActionEvent { get; set; }
        public bool? Disabled { get; set; }
    }
}

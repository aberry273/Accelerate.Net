using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Models.UI.Components
{
    public class ModalForm
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string Event { set; get; }
        public string Component { set; get; }
        public AjaxForm Form { get; set; }
    }
}

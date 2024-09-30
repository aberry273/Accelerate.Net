using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Models.UI.Components
{
    public enum PostbackType
    {
        GET, POST, PUT, DELETE
    }
    public class Form
    {
        public Form() { }
        public PostbackType Type { get; set; }
        public string Method => Enum.GetName(Type);
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Text { get; set; }
        public string Response { get; set; }
        public string Class { get; set; }
        public List<FormField> Fields { get; set; }
        public string Label { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Content.Models.Data
{
    public class ContentPostFormatData
    {
        public List<string>? items { get; set; }
        public string? style { get; set; }
        public string? text { get; set; }
    }
    public class ContentPostFormatItem
    {
        public string id { get; set; }
        public string type { get; set; }
        public ContentPostFormatData data { get; set; }
    }
}

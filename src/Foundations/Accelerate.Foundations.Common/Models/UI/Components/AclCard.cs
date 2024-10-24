using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Models.UI.Components
{
    public struct AclCard
    {
        public string Href { get; set; }
        [JsonProperty("img")]
        public string Image { get; set; }
        public string Label { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Text { get; set; }
        public string Subtext { get; set; }
    }
}

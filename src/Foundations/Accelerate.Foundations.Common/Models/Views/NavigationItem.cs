using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Models.Views
{
    public class NavigationItem
    {
        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("img")]
        public string Image { get; set; }

        [JsonProperty("tooltip")]
        public string Tooltip { get; set; }
    }
}

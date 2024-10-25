using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Models.Views
{
    public class NavigationGroup
    {
        public string Title { get; set; }

        public string Subtitle { get; set; }

        [JsonProperty("img", NullValueHandling = NullValueHandling.Ignore)]
        public string Image { get; set; }

        [JsonProperty("items")]
        public List<NavigationItem> Items { get; set; }

    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Models.Views
{
    public class NavigationAvatarDropdown
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }

        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        public string Image { get; set; }

        [JsonProperty("groups")]
        public List<NavigationGroup> Groups { get; set; }

    }
    public class NavigationBar : NavigationGroup
    {
        public string Logo { get; set; }

        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("primaryItems")]
        public List<NavigationItem> PrimaryItems { get; set; }

        [JsonProperty("secondaryItems")]
        public List<NavigationItem> SecondaryItems { get; set; }
        public NavigationAvatarDropdown Dropdown { get; set; }

        public bool Authenticated { get; set; }
    }
}

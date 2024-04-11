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

        [JsonProperty("img", NullValueHandling = NullValueHandling.Ignore)]
        public string Image { get; set; }

        [JsonProperty("items")]
        public List<NavigationItem> Items { get; set; }
    }
    public class NavigationBar : NavigationGroup
    {
        public NavigationAvatarDropdown Dropdown { get; set; }
        public bool Authenticated { get; set; }
    }
}

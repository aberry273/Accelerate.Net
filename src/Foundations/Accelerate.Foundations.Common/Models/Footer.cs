using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Models
{
    public class Footer
    {
        public string Title { get; set; }

        public string Subtitle { get; set; }

        public NavigationGroup Logo { get; set; } = new NavigationGroup();

        public NavigationGroup Contact { get; set; } = new NavigationGroup();

        public List<NavigationItem> Items { get; set; } = new List<NavigationItem>();
    }
}

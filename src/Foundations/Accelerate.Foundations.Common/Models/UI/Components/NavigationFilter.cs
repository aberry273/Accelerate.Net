using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Models.UI.Components
{
    public enum NavigationFilterType
    {
        Radio, Checkbox, Select
    }
    public class NavigationFilter
    {
        public string Name { get; set; }
        public NavigationFilterType FilterType { get; set; }
        public string Type
        {
            get
            {
                return Enum.GetName(this.FilterType);
            }
        }
        public List<string> Values { get; set; }
        public List<string> Selected { get; set; }
    }
}

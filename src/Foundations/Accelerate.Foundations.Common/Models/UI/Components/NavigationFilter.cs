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
    public class NavigationFilterValue
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public long? Count { get; set; }
    }
    public class NavigationFilterItem
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public NavigationFilterType FilterType { get; set; }
        public string Type
        {
            get
            {
                return Enum.GetName(this.FilterType);
            }
        } 
        public List<NavigationFilterValue> Values { get; set; }
        public List<string> Selected { get; set; }
    }
    public class NavigationFilter
    {
        public List<NavigationFilterItem> Filters { get; set; }
        public NavigationFilterItem Sort { get; set; }
        public NavigationFilterItem SortBy { get; set; }
    }
}

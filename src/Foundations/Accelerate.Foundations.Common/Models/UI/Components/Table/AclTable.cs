using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Models.UI.Components.Table
{
    public class AclTableRow
    {
        public List<List<string>> Values { get; set; }
    }
    public class AclTableRow<T>
    {
        public List<T> Values { get; set; }
    }
    public enum AclTableHeaderType
    {
        String, Link, Number, Date
    }
    public class AclTableHeader
    {
        public string? HeaderType
        {
            get
            {
                return Enum.GetName<AclTableHeaderType>(this.Type);
            }
        }
        public AclTableHeaderType Type { get; set; }
        public dynamic Data { get; set; }
        public string Class { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
    }
    public class AclTable
    {
        public List<AclTableHeader> Headers { get; set; }
        public List<AclTableRow> Items { get; set; }
    }
    public class AclTable<T>
    {
        public List<AclTableHeader> Headers { get; set; }
        public List<T> Items { get; set; }
    }

    public class AclAjaxTable<T> : AclTable<T>
    {
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int Pages { get; set; }
        public string Url { get; set; }
    }
}

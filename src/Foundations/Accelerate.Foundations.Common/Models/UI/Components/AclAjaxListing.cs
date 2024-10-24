using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Models.UI.Components
{
    public class AclListing<T>
    {
        public List<T> Items { get; set; }
    }

    public class AclAjaxListing<T> : AclListing<T>
    {
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int Pages { get; set; }
        public string PostbackUrl { get; set; }
    }
}

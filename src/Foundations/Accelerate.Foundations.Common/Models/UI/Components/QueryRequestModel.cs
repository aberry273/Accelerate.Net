using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Models.UI.Components
{
    public class QueryRequestModel<T>
    {
        public T? Query { get; set; }
        public int CurrentPage { get; set; }
        public int? ItemsPerPage { get; set; }
        public int? Pages { get; set; }
    }
}

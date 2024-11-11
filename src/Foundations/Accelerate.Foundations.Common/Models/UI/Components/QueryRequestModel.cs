using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Models.UI.Components
{
    public class QueryRequestModel<T>
    {
        [Required]
        public T Query { get; set; }
        [DefaultValue(0)]
        public int? Page { get; set; }
        [DefaultValue(10)]
        public int? PageSize { get; set; }
    }
}

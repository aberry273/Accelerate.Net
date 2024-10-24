using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Mediator.Models
{
    public class BaseResponsePagination<T> : BaseResponse<T>
    {
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int Total { get; set; }
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Models
{
    public interface IEntityViewModel
    {
        public Guid UserId { get; set; }
    }
}

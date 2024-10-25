using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Database.Models
{
    public interface IBaseEntity
    {
        Guid Id { get; set; }
        DateTime CreatedOn { get; set; }
        DateTime? UpdatedOn { get; set; }
    }
}

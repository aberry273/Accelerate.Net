using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Settlements.Models.Entities
{
    public class SettlementsBaseEntity : BaseEntity
    {
        public SettlementsStatusEnum Status { get; set; }
    }
}

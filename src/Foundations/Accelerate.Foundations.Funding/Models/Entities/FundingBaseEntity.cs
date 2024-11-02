using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Funding.Models.Entities
{
    public class FundingBaseEntity : BaseEntity
    {
        public FundingStatusEnum Status { get; set; }
    }
}

using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Kyc.Models.Entities
{
    public class KycBaseEntity : BaseEntity
    {
        public KycStatusEnum Status { get; set; }
        // Address
        public string? Reference { get; set; }
        public string? ExternalReference { get; set; }
        public string? ExternalProvider { get; set; }
    }
}

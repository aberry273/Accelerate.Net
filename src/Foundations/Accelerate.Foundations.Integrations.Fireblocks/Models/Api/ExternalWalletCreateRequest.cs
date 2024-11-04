using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Integrations.Fireblocks.Models.Api
{
    public class ExternalWalletCreateRequest
    {
        public string Name { get; set; }
        public string customerRefId { get; set; }
    }
}

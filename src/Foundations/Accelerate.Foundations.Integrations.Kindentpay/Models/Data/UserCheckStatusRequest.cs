using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Integrations.Kindentpay.Models.Data
{
    public class UserCheckStatusRequest
    {
        public string Key { get; set; }
        public string Token { get; set; }
        public decimal Amount { get; set; }
        public string Agent_Id { get; set; }
    }
}

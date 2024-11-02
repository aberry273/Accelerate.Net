using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Integrations.Kindentpay.Models.Data
{
    public class PayoutResponse
    {
        public string Txn_Id { get; set; }
        public decimal Amount { get; set; }
        public string Agent_Id { get; set; }
        public string Status { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Integrations.Kindentpay.Models.Data
{
    public class BalanceEnquriryData
    {
        public string Name { get; set; }
        public decimal Wallet_Balance { get; set; }
    }
    public class BalanceEnquiryResponse
    {
        public string Status { get; set; }
        public string Msg { get; set; }
        public BalanceEnquriryData Data { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Integrations.Kindentpay.Models.Data
{
    public class PayoutRequest
    {
        public string Phone { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string Account { get; set; }
        public string Bank_Name { get; set; }
        public string IFSC { get; set; }
        public string Email { get; set; }
        public string Agent_Id { get; set; }
        public string Key { get; set; }
        public string Token { get; set; }
    }
}

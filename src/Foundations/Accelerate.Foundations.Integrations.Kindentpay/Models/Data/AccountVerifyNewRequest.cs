using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Integrations.Kindentpay.Models.Data
{
    public class AccountVerifyNewRequest
    {
        public string Account_Number { get; set; }
        public string IFSC_Code { get; set; }
        public decimal Amount { get; set; }
        public string Agent_Id { get; set; }
        public string User_Id { get; set; }
        public string Token { get; set; }
    }
}

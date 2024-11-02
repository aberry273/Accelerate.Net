using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Integrations.Kindentpay.Models.Data
{
    public class AccountVerifyNewResponseData
    {
        public string Txn_Id { get; set; }
        public string Status { get; set; }
        public string Status_Code { get; set; }
        public string Status_Msg { get; set; }
        public string Order_Id { get; set; }
        public string BeneficiaryName { get; set; }
    }
    public class AccountVerifyNewResponse
    {
        public string Status { get; set; }
        public string Msg { get; set; }
        public AccountVerifyNewResponseData Data { get; set; }
    }
}

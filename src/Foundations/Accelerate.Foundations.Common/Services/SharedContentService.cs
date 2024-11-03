using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Services
{
    public class SharedContentService : ISharedContentService
    {
        public  List<dynamic> GetCustomerTypes()
        {
            return new List<dynamic>(){
                "Individual",
                "Organisation"
            };
        }
        public  List<dynamic> GetIndustries()
        {
            return new List<dynamic>(){
                "Fintech"
            };
        }
        public  List<dynamic> GetCountryCodes()
        {
            return new List<dynamic>(){
                "UAE",
                "IND",
                "AU",
                "NZ"
            };
        }
        public List<dynamic> GetBusinessAccountTypes()
        {
            return new List<dynamic>(){
                "SoleTrader",
                "Partnership",
                "Undefined",
                "BV",
                "CVN",
                "CVOA",
                "EEN",
                "MTS",
                "NV",
                "SE",
                "VOF",
                "CCORP",
                "TrustEstate",
                "SoleProprietorship",
                "SingleMemberLimitedLiability",
                "LimitedLiability",
                "PublicallyListedCompany",
                "PrivateCompany",
                "NonForProfit",
                "NRO",
                "NRE",
                "Individual",
            };
        }
    }
}

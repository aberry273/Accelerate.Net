using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Services
{
    public interface ISharedContentService
    {
        List<dynamic> GetCustomerTypes();
        List<dynamic> GetIndustries();
        List<dynamic> GetCountryCodes();
        List<dynamic> GetBusinessAccountTypes();
    }
}

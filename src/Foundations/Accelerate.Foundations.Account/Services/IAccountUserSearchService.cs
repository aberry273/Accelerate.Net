using Accelerate.Foundations.Account.Models.Data;
using Accelerate.Foundations.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Account.Services
{
    public interface IAccountUserSearchService
    {
        Task<AccountSearchResults> SearchUsers(RequestQuery Query);
    }
}

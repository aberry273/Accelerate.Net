using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Account.Models.Data
{
    public class AccountSearchResults
    {
        public List<AccountUserDocument> Users { get; set; } = new List<AccountUserDocument>();
    }
}

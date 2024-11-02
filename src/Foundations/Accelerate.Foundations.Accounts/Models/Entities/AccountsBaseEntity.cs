using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Accounts.Models.Entities
{
    public class AccountsBaseEntity : BaseEntity
    {
        public AccountsStatusEnum Status { get; set; }
    }
}

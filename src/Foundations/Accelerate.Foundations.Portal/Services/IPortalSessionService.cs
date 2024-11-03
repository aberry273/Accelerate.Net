using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Portal.Services
{
    public interface IPortalSessionService
    {
        string? TryGetSelectedAccountId();
        void SetAccountSession(Guid accountId);
    }
}

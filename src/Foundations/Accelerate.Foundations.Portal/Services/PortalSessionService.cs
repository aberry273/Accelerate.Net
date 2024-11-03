using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Accelerate.Foundations.Portal.Services
{

    public class PortalSessionService : IPortalSessionService
    {
        protected readonly IActionContextAccessor _accessor;
        protected readonly IUrlHelperFactory _urlHelperFactory;
        protected IUrlHelper _urlHelper
        {
            get
            {
                return _urlHelperFactory.GetUrlHelper(_accessor.ActionContext);
            }
        }
        public PortalSessionService(
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccessor
        )
        {
            _accessor = actionContextAccessor;
            _urlHelperFactory = urlHelperFactory;
        }
        public string? TryGetSelectedAccountId()
        {
            if (_urlHelper?.ActionContext?.HttpContext?.Session == null) return null;
            return _urlHelper?.ActionContext?.HttpContext.Session.GetString(Constants.Keys.SessionAccountKey);
        }
        public void ClearSessionAccount()
        {
            if (_urlHelper?.ActionContext?.HttpContext?.Session == null) return;
            _urlHelper?.ActionContext?.HttpContext?.Session?.Remove(Constants.Keys.SessionAccountKey);
        }
        public void SetAccountSession(Guid accountId)
        {
            if (_urlHelper?.ActionContext?.HttpContext?.Session == null) return;
            _urlHelper?.ActionContext?.HttpContext?.Session?.SetString(Constants.Keys.SessionAccountKey, accountId.ToString());
        }

    }
}

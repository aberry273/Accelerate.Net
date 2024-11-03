using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.UI.Components.Table;
using Accelerate.Foundations.Common.Services;
using Elastic.Clients.Elasticsearch;
using System.Collections.Generic;
using Accelerate.Foundations.Portal.Services;
using Accelerate.Features.Accounts.Services;
using Accelerate.Foundations.Accounts.Models.Entities;

namespace Accelerate.Features.Admin.Services
{
    public class AccountsBusinessAccountViewService : AccountsBaseEntityViewService<AccountsBusinessEntity>
    {
        public AccountsBusinessAccountViewService(
            IMetaContentService metaContent,
            IPortalContentService portalContentService)
            : base(metaContent, portalContentService)
        {
            EntityName = "Accounts";
        } 
    }
}

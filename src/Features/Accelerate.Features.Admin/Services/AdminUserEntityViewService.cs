using Accelerate.Features.Admin.Models.Views; 
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.UI.Components.Table;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Operations.Models.Entities;
using Elastic.Clients.Elasticsearch;
using System.Collections.Generic;

namespace Accelerate.Features.Admin.Services
{
    public class AdminUserEntityViewService : AdminBaseEntityViewService<AccountUser>
    {
        IEntityService<AccountUser> _accountUser;
        public AdminUserEntityViewService(
            IEntityService<AccountUser> accountUser,
            IMetaContentService metaContent)
            : base(metaContent)
        {
            _accountUser = accountUser;
            EntityName = "User";
        }
        public override string GetEntityName(AccountUser item)
        {
            return item.UserName;
        }
    }
}

﻿using Accelerate.Features.Admin.Services;
using Accelerate.Foundations.Account.Attributes;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Account.Services;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Content.Services;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Operations.Models.Entities;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Aggregations;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading;
using Twilio.TwiML.Voice;

namespace Accelerate.Features.Content.Controllers
{
    [Route("Admin/[controller]")]
    public class UsersController : BaseAdminController<AccountUser>
    {
        const string channelName = "User";
        public UsersController(
            SignInManager<AccountUser> signInManager,
            UserManager<AccountUser> userManager,
            IEntityService<AccountProfile> profileService,
            IMetaContentService contentService,
            IEntityService<AccountUser> entityService,
            IAdminBaseEntityViewService<AccountUser> contentViewService,
            IElasticService<ContentPostDocument> postSearchService,
            IContentPostElasticService contentElasticSearchService)
            : base(channelName, signInManager, userManager, profileService, contentService, entityService, contentViewService, postSearchService, contentElasticSearchService)
        {
        }

    }
}
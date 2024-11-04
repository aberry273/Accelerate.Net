using Accelerate.Features.Admin.Services;
using Accelerate.Foundations.Users.Attributes;
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Aggregations;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading;
using Twilio.TwiML.Voice;
using Accelerate.Foundations.Rates.Models.Entities;
using Accelerate.Features.Rates.Services;
using Accelerate.Foundations.Users.Services;
using Accelerate.Foundations.Portal.Services;

namespace Accelerate.Features.Content.Controllers
{ 
    [Route("Rates/[controller]")]
    public class QuoteController : BaseRatesController<RatesConversionQuoteEntity>
    {
        const string channelName = "Quote";
        public QuoteController(
            SignInManager<UsersUser> signInManager,
            UserManager<UsersUser> userManager,
            IUsersUserService userService,
            IEntityService<UsersProfile> profileService,
            IPortalSessionService portalSessionService,
            IMetaContentService contentService,
            IEntityService<RatesConversionQuoteEntity> entityService,
            IRatesBaseEntityViewService<RatesConversionQuoteEntity> contentViewService)
            : base(channelName, signInManager, userManager, userService, profileService, portalSessionService, contentService, entityService, contentViewService)
        {
        }
    } 
}
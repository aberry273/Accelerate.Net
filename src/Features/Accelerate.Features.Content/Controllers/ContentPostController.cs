using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.Database.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Features.Content.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentPostController : BaseApiController<ContentPostEntity>
    {
        UserManager<AccountUser> _userManager;
        IMetaContentService _contentService;
        public ContentPostController(
            IMetaContentService contentService,
            IEntityService<ContentPostEntity> service,
            UserManager<AccountUser> userManager) : base(service)
        {
            _userManager = userManager;
            _contentService = contentService;
        }
        private BasePage CreateBaseContent(AccountUser user)
        {
            var profile = user != null ? new UserProfile()
            {
                Username = user.UserName,
            } : null;
            return _contentService.CreatePageBaseContent(profile);
        }
    }
}
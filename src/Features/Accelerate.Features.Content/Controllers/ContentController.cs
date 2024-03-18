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
    public class ContentController : BaseController
    {
        UserManager<AccountUser> _userManager;
        IMetaContentService _contentService;
        public ContentController(
            IMetaContentService service,
            IEntityService<ContentPostEntity> postService,
            UserManager<AccountUser> userManager) : base(service)
        {
            _userManager = userManager;
            _contentService = service;
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
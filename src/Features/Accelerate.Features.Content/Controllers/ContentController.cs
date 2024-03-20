using Accelerate.Features.Content.Models.Views;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.Views;
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
        public async Task<IActionResult> Feed()
        {
            var user = await _userManager.GetUserAsync(this.User);
            var model = CreateBaseContent(user);
            var viewModel = new FeedPage(model);
            viewModel.UserId = user.Id;
            return View(viewModel);
        }
    }
}
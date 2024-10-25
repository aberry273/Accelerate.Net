﻿using Accelerate.Foundations.Account.Attributes;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Features.Content.Controllers;
using Accelerate.Foundations.Integrations.AzureStorage.Services;
using Accelerate.Foundations.Media.Services;

namespace Accelerate.Projects.App.Controllers
{
    //[Authorize]
    public class MediaController : BaseController
    {
        UserManager<AccountUser> _userManager;
        IMetaContentService _contentService;
        IMediaService _mediaService;
        public MediaController(
            UserManager<AccountUser> userManager,
            IMetaContentService contentService,
            IMediaService mediaService)
            : base(contentService)
        {
            _mediaService = mediaService;
            _contentService = contentService;
            _userManager = userManager;
        }
        public async Task<ActionResult> Files(string id, [FromQuery]int? w, [FromQuery] int? h)
        {
            try
            {
                var file = await _mediaService.GetPublicLocation(id);
                if (file == null) return null;
                if (w == null && h == null)
                    return new FileStreamResult(new FileStream(file, FileMode.Open), "image/png"); ;

                var ignoreAspectRatio = h != null && w != null;
                var width = w != null ? w.GetValueOrDefault() : h.GetValueOrDefault();
                var height = h != null ? h.GetValueOrDefault() : width;
                var resizedPath = _mediaService.GetParameterPath(file, height, width);

                if (!_mediaService.FileExists(resizedPath))
                {
                    resizedPath = _mediaService.ResizeImage(file, height, width, ignoreAspectRatio);
                }

                return new FileStreamResult(new FileStream(resizedPath, FileMode.Open), "image/png");
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}

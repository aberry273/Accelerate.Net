
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.Data;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Media.Models.Entities;
using Accelerate.Foundations.Media.Services;
using MassTransit.DependencyInjection;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.StaticFiles;
using static Accelerate.Foundations.Database.Constants.Exceptions;
using MassTransit.Transports;
using Accelerate.Foundations.Media.EventBus;

namespace Accelerate.Features.Content.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaBlobController : BaseApiServiceController<MediaBlobEntity>
    {
        readonly Bind<IMediaBlobEventBus, IPublishEndpoint> _publishEndpoint;
        UserManager<AccountUser> _userManager;
        IMediaService _mediaService;
        IMetaContentService _contentService;
        public MediaBlobController(
            IMetaContentService contentService,
            Bind<IMediaBlobEventBus, IPublishEndpoint> publishEndpoint,
            IEntityService<MediaBlobEntity> service,
            IMediaService mediaService,
            UserManager<AccountUser> userManager) : base(service)
        {
            _publishEndpoint = publishEndpoint;
            _userManager = userManager;
            _contentService = contentService;
            _mediaService = mediaService;
        }
        [HttpPost("image")]
        [Consumes("multipart/form-data")]
        public async Task<IStatusCodeActionResult> UploadImage([FromForm] EntityFile model)
        {
            try
            {
                //TOOD: Move to mediaService function
                if (model.File != null)
                { 
                    var user = await _userManager.FindByIdAsync(model.UserId.ToString());
                    // upload file
                    var guid = Guid.NewGuid();
                    var fileId = await _mediaService.UploadFile(guid, model.UserId.ToString(), model.File);
                    var url = _mediaService.GetFileUrl(fileId);
                    // create entity
                    var entity = new MediaBlobEntity()
                    {
                        Id = guid,
                        FilePath = url,
                        Name = model.File.FileName,
                        UserId = model.UserId
                    };

                    var item = await this.Post(entity);

                    return Ok(item);
                }
                return NotFound();

            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }

        private string GetFileType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(fileName, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }

        protected override async Task PostCreateSteps(MediaBlobEntity obj)
        {
            await _publishEndpoint.Value.Publish(new CreateDataContract<MediaBlobEntity>() { Data = obj });
        }
        protected override void UpdateValues(MediaBlobEntity from, dynamic to)
        {
            from.Status = to.Status;
            from.Name = to.Name;
            from.Tags = to.Tags;
            from.FilePath = to.FilePath;
        }
        protected override async Task PostUpdateSteps(MediaBlobEntity obj)
        {
            await _publishEndpoint.Value.Publish(new UpdateDataContract<MediaBlobEntity>() { Data = obj });
        }
        protected override async Task PostDeleteSteps(MediaBlobEntity obj)
        {
            await _publishEndpoint.Value.Publish(new DeleteDataContract<MediaBlobEntity>() { Data = obj });
        }
    }
}
using Accelerate.Foundations.Account.EventBus;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Models.Data;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Accelerate.Foundations.Integrations.AzureStorage.Services;
using Accelerate.Foundations.Integrations.Contracts;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Media.Services;
using Accelerate.Foundations.Websockets.Hubs;
using MassTransit;
using MassTransit.DependencyInjection;
using MassTransit.Transports;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.StaticFiles;
using static Accelerate.Foundations.Database.Constants.Exceptions;

namespace Accelerate.Features.Account.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountProfileController : BaseApiController<AccountProfile>
    { 
        UserManager<AccountUser> _userManager;
        IMetaContentService _contentService;
        IBlobStorageService _blobStorageService;
        IMediaService _mediaService;
        readonly Bind<IAccountBus, IPublishEndpoint> _publishEndpoint;
        //readonly Bind<IContentChannelBus, IPublishEndpoint> _publishEndpoint;
        //IElasticService<ContentChannelDocument> _searchService;
        //IEntityService<ContentPostEntity> _postService;
        public AccountProfileController(
            IMetaContentService contentService,
            IBlobStorageService blobStorageService,
            IMediaService mediaService,
            IEntityService<AccountProfile> service,
            Bind<IAccountBus, IPublishEndpoint> publishEndpoint,
            //IEntityService<ContentPostEntity> postService,
           // Bind<IContentChannelBus, IPublishEndpoint> publishEndpoint,
            //IElasticService<ContentChannelDocument> searchService,
            UserManager<AccountUser> userManager) : base(service)
        {
            _publishEndpoint = publishEndpoint;
            _userManager = userManager;
            _contentService = contentService;
            _blobStorageService = blobStorageService;
            _mediaService = mediaService;
            //_searchService = searchService;
            //_postService = postService;
        }


        protected override async Task PostCreateSteps(AccountProfile obj)
        {
            //await _publishEndpoint.Value.Publish(new CreateDataContract<AccountProfile>() { Data = obj });
        }
        protected override async Task PostUpdateSteps(AccountProfile obj)
        {
        }
        protected override async Task PostDeleteSteps(AccountProfile obj)
        {
            //await _publishEndpoint.Value.Publish(new DeleteDataContract<AccountProfile>() { Data = obj });
        }
        protected override void UpdateValues(AccountProfile from, dynamic to)
        {
            from.Firstname = to.Firstname;
            from.Lastname = to.Lastname;

            from.Address = to.Address;
            from.Position = to.Position;
            from.Postcode = to.Postcode;
            from.City = to.City;
            from.Country = to.Country;
            from.Title = to.Title;
            from.Suburb = to.Suburb;

            from.Category = to.Firstname;
            from.Tags = to.Tags;
        }

        [HttpPut("{id}/image")]
        [Consumes("multipart/form-data")]
        public async Task<IStatusCodeActionResult> UpdateProfile([FromRoute] Guid id, [FromForm] EntityFile model)
        {
            try
            {
                if (model.File != null)
                {
                    var type = GetFileType(model.File.FileName);
                    var profile = base._service.Get(id);
                    var user = await _userManager.FindByIdAsync(profile.UserId.ToString());
                    // If image exists, delete the file
                    /*
                    if(profile.Image != null)
                    {
                        var privatePath = _blobStorageService.GetPrivateLocation(profile.Image);
                        if (privatePath != null)
                            await this._blobStorageService.DeleteAsync(privatePath);
                    }
                    var filePath = await this._mediaService.UploadFile(id.ToString(), model.File);
                    var publicPath = await _blobStorageService.GetPublicLocation(filePath);
                    */
                    var fileId = await _mediaService.UploadFile(id.ToString(), model.File);
                    var url = _mediaService.GetFileUrl(fileId);
                    profile.Image = url;
                    var result = await base._service.Update(profile);

                    //TODO: Create profile pipelines instead of running the user pipeline on the profile
                    await _publishEndpoint.Value.Publish(new UpdateDataContract<AccountUser>() { Data = user });
                    return Ok(profile);
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
    }
}
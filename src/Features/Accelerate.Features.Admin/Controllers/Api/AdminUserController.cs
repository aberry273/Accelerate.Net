using Accelerate.Foundations.Content.EventBus;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Accelerate.Foundations.Integrations.Contracts;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Websockets.Hubs;
using MassTransit;
using MassTransit.DependencyInjection;
using MassTransit.Transports;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using static Accelerate.Foundations.Database.Constants.Exceptions;
using Accelerate.Foundations.Operations.Models.Entities;
using Accelerate.Foundations.Account.EventBus;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Features.Admin.Models.Data;
using Accelerate.Foundations.Account.Models;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.StaticFiles;
using Accelerate.Foundations.Common.Models.Data;
using Accelerate.Foundations.Media.Services;
using Elastic.Clients.Elasticsearch.Core.Search;

namespace Accelerate.Features.Admin.Controllers.Api
{ 

    public class AdminUserController : BaseApiServiceController<AccountUser>
    {
        UserManager<AccountUser> _userManager;
        IMetaContentService _contentService;  
        IEntityService<ContentPostEntity> _postService;
        IEntityService<AccountProfile> _profileService;
        IElasticService<AccountUserDocument> _searchService;
        IMediaService _mediaService;
        Bind<IAccountBus, IPublishEndpoint> _publishEndpoint;
        public AdminUserController(
            IMetaContentService contentService,
            IEntityService<AccountUser> service,
            IEntityService<ContentPostEntity> postService,
            IEntityService<AccountProfile> profileService,
            IElasticService<AccountUserDocument> searchService,
            IMediaService mediaService,
            Bind<IAccountBus, IPublishEndpoint> publishEndpoint,
            UserManager<AccountUser> userManager) : base(service)
        { 
            _userManager = userManager;
            _contentService = contentService; 
            _postService = postService;
            _profileService = profileService;
            _publishEndpoint = publishEndpoint;
            _searchService = searchService;
            _mediaService = mediaService;
        }
        protected override async Task PostCreateSteps(AccountUser obj)
        {
            await _publishEndpoint.Value.Publish(new CreateDataContract<AccountUser>()
            {
                Data = obj,
                UserId = obj.Id
            });
        }
        protected override async Task PostUpdateSteps(AccountUser obj)
        {
            await _publishEndpoint.Value.Publish(new UpdateDataContract<AccountUser>()
            {
                Data = obj,
                UserId = obj.Id
            });
        }
        protected override async Task PostDeleteSteps(AccountUser obj)
        {
            await _publishEndpoint.Value.Publish(new DeleteDataContract<AccountUser>()
            {
                Data = obj,
                UserId = obj.Id
            });
        }
        protected override void UpdateValues(AccountUser from, dynamic to)
        {
            from.Domain = to.Domain;
            from.UserName = to.UserName;
            from.Status = to.Status;
            from.Email = to.Email;
            from.PhoneNumber = to.PhoneNumber;
        }

        [HttpPut]
        [Route("{id}/index")]
        public virtual async Task<IActionResult> Index([FromRoute] Guid id, [FromBody] AdminUserIndexApiRequest request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    return NotFound();
                }
                var profile = request.ProfileId != null ? _profileService.Get(request.ProfileId.GetValueOrDefault()) : null;

                var indexModel = new AccountUserDocument()
                {
                    CreatedOn = user.CreatedOn,
                    UpdatedOn = user.UpdatedOn,
                    Domain = user.Domain,
                    Id = user.Id,
                    Username = user.UserName,
                    Firstname = profile?.Firstname,
                    Lastname = profile?.Lastname,
                    Image = profile?.Image,
                };

                var result = await _searchService.IndexDocument(indexModel);
                return Ok(result.Result);
            }
            catch (Exception ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
                return BadRequest();
            }
        }

        /// <summary>
        /// TODO: Share same functions form accountuser api controlller
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        /// 
        [HttpPost("{id}/profile")]
        public virtual async Task<IActionResult> CreateProfile([FromRoute] Guid id, [FromBody] AccountProfile obj)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    return NotFound();
                }
                var profile = user.AccountProfileId != null ? _profileService.Get(user.AccountProfileId.GetValueOrDefault()) : null;

                if (profile != null)
                {
                    return Ok();
                }
                var profileModel = new AccountProfile();
                UpdateValues(profileModel, obj);
                profileModel.UserId = user.Id;
                profileModel.UpdatedOn = DateTime.Now;
                var guid = await _profileService.CreateWithGuid(profileModel);
                user.AccountProfileId = guid;

                await _userManager.UpdateAsync(user);
                //To override
                var updatedEntity = _profileService.Get(id);

                var indexModel = new AccountUserDocument()
                {
                    CreatedOn = user.CreatedOn,
                    UpdatedOn = user.UpdatedOn,
                    Domain = user.Domain,
                    Id = user.Id,
                    Username = user.UserName,
                    Firstname = profileModel?.Firstname,
                    Lastname = profileModel?.Lastname,
                    Image = profileModel?.Image,
                };

                var result = await _searchService.IndexDocument(indexModel);

                return Ok(new
                {
                    message = "Updated Successfully",
                    id = id
                });
            }
            catch (Exception ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
                return BadRequest();
            }
        }
        protected void UpdateValues(AccountProfile from, dynamic to)
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


        [HttpPut("profile/{id}/image")]
        [Consumes("multipart/form-data")]
        public async Task<IStatusCodeActionResult> UpdateUserProfile([FromRoute] Guid id, [FromForm] EntityFile model)
        {
            try
            {
                if (model.File != null)
                {
                    var type = GetFileType(model.File.FileName);
                    var profile = _profileService.Get(id);
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
                    var result = await _profileService.Update(profile);

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
        [HttpPost("deactivate/delete")]
        public async Task<IStatusCodeActionResult> DeleteDeactivated([FromBody] DeactivateUserApiRequest model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.UserId.ToString());

                if (user == null)
                {
                    return NotFound();
                }
                if (user.Status != AccountUserStatus.Deactivated)
                {
                    return Problem("User not deactivated");
                }
                var deactivatedUsername = user.Id.ToString();
                user.Status = AccountUserStatus.Deleted;
                user.Email = deactivatedUsername + "@deleted.parot.app";
                user.UserName = deactivatedUsername;
                user.UpdatedOn = DateTime.Now;
                user.AccountProfileId = Guid.Empty;
                await _userManager.UpdateAsync(user);
                var logins = await _userManager.GetLoginsAsync(user);
                foreach (var login in logins)
                {
                    await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
                }
                var profile = _profileService.Get(user.AccountProfileId.GetValueOrDefault());
                if (profile != null)
                {
                    await _profileService.Delete(profile);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        } 
        [HttpPost("deactivate")]
        public async Task<IStatusCodeActionResult> DeactivateUser([FromBody] DeactivateUserApiRequest model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.UserId.ToString());

                if (user == null)
                {
                    return NotFound();
                }
                user.Status = AccountUserStatus.Deactivated;
                await _userManager.UpdateAsync(user);
                await PostUpdateSteps(user);

                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }
        [HttpPost("reactivate")]
        public async Task<IStatusCodeActionResult> ReactivateUser([FromBody] DeactivateUserApiRequest model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.UserId.ToString());

                if (user == null)
                {
                    return NotFound();
                }
                user.Status = AccountUserStatus.Active;
                await _userManager.UpdateAsync(user);
                await PostUpdateSteps(user);

                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.ToString());
            }
        }
    }
}
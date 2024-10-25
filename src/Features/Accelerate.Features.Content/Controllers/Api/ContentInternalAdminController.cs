using Accelerate.Foundations.Content.EventBus;
using Accelerate.Features.Content.Services;
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
using Accelerate.Features.Content.Models.Data;

namespace Accelerate.Features.Content.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentInternalAdminController : ControllerBase
    {
        IEntityService<ContentPostEntity> _servicePosts { get; set; }
        IEntityService<ContentPostActivityEntity> _servicePostActivities { get; set; }
        IEntityService<ContentPostActionsEntity> _servicePostActions { get; set; }
        IEntityService<ContentPostQuoteEntity> _servicePostQuotes { get; set; }
        IEntityService<ContentPostMediaEntity> _servicePostMedia { get; set; }
        IEntityService<ContentPostActionsSummaryEntity> _serviceActionsSummary { get; set; }
        IEntityService<ContentPostParentEntity> _servicePostParents { get; set; }
        IEntityService<ContentPostChannelEntity> _servicePostChannel { get; set; }
        IEntityService<ContentPostLinkEntity> _servicePostLink { get; set; }
        IEntityService<ContentPostSettingsEntity> _servicePostSettings { get; set; }
        IEntityService<ContentPostTaxonomyEntity> _servicePostTaxonomy { get; set; }
        IEntityService<ContentPostSettingsPostEntity> _servicePostSettingPost { get; set; }
        IEntityService<ContentChannelEntity> _serviceChannels { get; set; }
        //ES

        IElasticService<ContentPostDocument> _elasticPosts;
        IElasticService<ContentPostActivityDocument> _elasticPostActivities;
        IElasticService<ContentPostActionsDocument> _elasticPostActions;
        IElasticService<ContentPostQuoteDocument> _elasticPostQuotes;
        IElasticService<ContentPostMediaDocument> _elasticPostMedia;
        IElasticService<ContentPostActionsSummaryDocument> _elasticActionsSummary;
        IElasticService<ContentPostSettingsDocument> _elasticPostSettings;
        IElasticService<ContentChannelDocument> _elasticChannels;
        public ContentInternalAdminController(
            //DB
            IEntityService<ContentPostEntity> servicePosts,
            IEntityService<ContentPostActivityEntity> servicePostActivities,
            IEntityService<ContentPostActionsEntity> servicePostActions,
            IEntityService<ContentPostQuoteEntity> servicePostQuotes,
            IEntityService<ContentPostMediaEntity> servicePostMedia,
            IEntityService<ContentPostActionsSummaryEntity> serviceActionsSummary,
            IEntityService<ContentPostParentEntity> servicePostParents,
            IEntityService<ContentPostChannelEntity> servicePostChannel,
            IEntityService<ContentPostLinkEntity> servicePostLink,
            IEntityService<ContentPostSettingsEntity> servicePostSettings,
            IEntityService<ContentPostTaxonomyEntity> servicePostTaxonomy,
            IEntityService<ContentPostSettingsPostEntity> servicePostSettingPost,
            IEntityService<ContentChannelEntity> serviceChannels,
            //ES

            IElasticService<ContentPostDocument> elasticPosts,
            IElasticService<ContentPostActivityDocument> elasticPostActivities,
            IElasticService<ContentPostActionsDocument> elasticPostActions,
            IElasticService<ContentPostQuoteDocument> elasticPostQuotes,
            IElasticService<ContentPostMediaDocument> elasticPostMedia,
            IElasticService<ContentPostActionsSummaryDocument> elasticActionsSummary,

            IElasticService<ContentPostSettingsDocument> elasticPostSettings,

            IElasticService<ContentChannelDocument> elasticChannels
            )
        {
            this._servicePosts = servicePosts;
            this._servicePostActivities = servicePostActivities;
            this._servicePostActions = servicePostActions;
            this._servicePostQuotes = servicePostQuotes;
            this._servicePostMedia = servicePostMedia;
            this._serviceActionsSummary = serviceActionsSummary;
            this._servicePostParents = servicePostParents;
            this._servicePostChannel = servicePostChannel;
            this._servicePostLink = servicePostLink;
            this._servicePostSettings = servicePostSettings;
            this._servicePostSettingPost = servicePostSettingPost;
            this._serviceChannels = serviceChannels;
            this._servicePostTaxonomy = servicePostTaxonomy;
            //ES

            this._elasticPosts = elasticPosts;
            this._elasticPostActivities = elasticPostActivities;
            this._elasticPostActions = elasticPostActions;
            this._elasticPostQuotes = elasticPostQuotes;
            this._elasticPostMedia = elasticPostMedia;
            this._elasticActionsSummary = elasticActionsSummary;
            this._elasticPostSettings = elasticPostSettings;
            this._elasticChannels = elasticChannels;
        }

        [Route("_cleardb/{clearId}")]
        [HttpPost]
        public IActionResult ClearDb([FromRoute] string clearId)
        {
            try
            {
                _servicePostActivities.ClearDatabase();
                _servicePostActions.ClearDatabase();
                _servicePostQuotes.ClearDatabase();
                _servicePostMedia.ClearDatabase();
                _servicePostLink.ClearDatabase();
                _servicePostParents.ClearDatabase();
                _servicePostChannel.ClearDatabase();
                _serviceActionsSummary.ClearDatabase();
                _serviceChannels.ClearDatabase();
                _servicePostTaxonomy.ClearDatabase();
                _servicePostSettingPost.ClearDatabase();
                _servicePostSettings.ClearDatabase();
                _servicePosts.ClearDatabase();
                //ES
                _elasticPosts.DeleteIndex();
                _elasticPostActivities.DeleteIndex();
                _elasticPostActions.DeleteIndex();
                _elasticPostQuotes.DeleteIndex();
                _elasticPostMedia.DeleteIndex();
                _elasticActionsSummary.DeleteIndex();
                _elasticPostSettings.DeleteIndex();
                _elasticChannels.DeleteIndex();
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
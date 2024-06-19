using Accelerate.Foundations.Content.EventBus;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Services;
using MassTransit.DependencyInjection;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Accelerate.Foundations.Common.Pipelines;
using Microsoft.AspNetCore.Mvc;
using static Accelerate.Foundations.Database.Constants.Exceptions;
using Accelerate.Foundations.Content.Hydrators;

namespace Accelerate.Foundations.Content.Services
{
    public class ContentPostService : IContentPostService
    {
        readonly Bind<IContentPostBus, IPublishEndpoint> _publishEndpoint;
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
        public ContentPostService(
            Bind<IContentPostBus, IPublishEndpoint> publishEndpoint,
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
            IEntityService<ContentChannelEntity> serviceChannels)
        {
            this._publishEndpoint = publishEndpoint;
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
        }


        #region Post

        public async Task<ContentPostEntity> Create(ContentPostEntity obj)
        {
            try
            {
                var id = await _servicePosts.CreateWithGuid(obj);

                if (id == null)
                {
                    return null;
                }
                //To override
                var entity = _servicePosts.Get(id.GetValueOrDefault());
                return entity;
            }
            catch (Exception ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
                throw ex;
            }
        }
        public async Task<ContentPostEntity> CreateWithPipeline(ContentPostEntity obj)
        {
            try
            {
                var id = await _servicePosts.CreateWithGuid(obj);

                if (id == null)
                {
                    return null;
                }
                //To override
                var entity = _servicePosts.Get(id.GetValueOrDefault());
                await this.RunCreatePipeline(entity);
                return entity;
            }
            catch (Exception ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
                throw ex;
            }
        }

        public async Task<ContentPostEntity> Update(Guid id, ContentPostEntity obj)
        {
            try
            {
                var entity = _servicePosts.Get(id);
                if (entity == null)
                {
                    return null;
                }
                obj.Hydrate(entity);
                entity.UpdatedOn = DateTime.Now;
                await _servicePosts.Update(entity);
                //To override
                var updatedEntity = _servicePosts.Get(id);
                await RunUpdatePipeline(updatedEntity);

                return updatedEntity;
            }
            catch (Exception ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
                throw ex;
            }
        }

        public async Task<int> Delete([FromRoute] Guid id)
        {
            try
            {
                var entity = _servicePosts.Get(id);
                if (entity == null)
                {
                    return -1;
                }
                var result = await _servicePosts.Delete(entity);

                await RunDeletePipeline(entity);
                return result;
            }
            catch (Exception ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
                throw ex;
            }
        }
        
        #endregion

        #region EventPipelines

        public async Task RunCreatePipeline(ContentPostEntity obj)
        {
            await _publishEndpoint.Value.Publish(new CreateDataContract<ContentPostEntity>()
            {
                Data = obj,
                Target = obj.ThreadId,
                UserId = obj.UserId
            });
        }
        public async Task RunUpdatePipeline(ContentPostEntity obj)
        {
            await _publishEndpoint.Value.Publish(new UpdateDataContract<ContentPostEntity>()
            {
                Data = obj,
                Target = obj.ThreadId,
                UserId = obj.UserId
            });
        }
        public async Task RunDeletePipeline(ContentPostEntity obj)
        {
            await _publishEndpoint.Value.Publish(new DeleteDataContract<ContentPostEntity>()
            {
                Data = obj,
                Target = obj.ThreadId,
                UserId = obj.UserId
            });
        }

        #endregion

        #region Parents
        public async Task<int> CreateParentPost(ContentPostEntity post, Guid parentId, List<Guid> ancestorIds)
        {
            var entity = new ContentPostParentEntity()
            {
                ContentPostId = post.Id,
                ParentId = parentId,
                ParentIdItems = ancestorIds
            };
            return await _servicePostParents.Create(entity);
        }

        public List<ContentPostEntity> GetReplies(Guid postId)
        {
            var postIds = _servicePostParents.Find(x => x.ParentId == postId).Select(x => x.ContentPostId).ToList();
            var posts = _servicePosts.Find(x => postIds.Contains(x.Id)).ToList();
            return posts;
        }
        public int GetReplyCount(Guid postId)
        {
            return _servicePostParents.Find(x => x.ParentId == postId).Count();
        }
        #region Taxonomy

        public async Task<Guid?> CreateTaxonomy(Guid postId, ContentPostTaxonomyEntity entity)
        {
           return await _servicePostTaxonomy.CreateWithGuid(entity);
        }
        public ContentPostTaxonomyEntity GetTaxonomy(Guid postId)
        {
            return _servicePostTaxonomy.Find(x => x.ContentPostId == postId)?.FirstOrDefault();
        }
        #endregion
        #region Settings

        public async Task<int> CreateSettings(Guid postId, ContentPostSettingsEntity entity)
        {
            var settingsGuid = await _servicePostSettings.CreateWithGuid(entity);
            var joinEntity = new ContentPostSettingsPostEntity()
            {
                ContentPostId = postId,
                ContentPostSettingsId = settingsGuid.GetValueOrDefault()
            };
            return await CreateSettingsJoin(joinEntity);
        }
        public async Task<int> CreateSettingsJoin(ContentPostSettingsPostEntity entity)
        {
            var settingsJoin = await _servicePostSettingPost.Create(entity);
            return settingsJoin;
        }
        public ContentPostSettingsEntity GetSettings(Guid postId)
        {
            var join = _servicePostSettingPost.Find(x => x.ContentPostId == postId)?.FirstOrDefault();
            if(join == null)
            {
                return null;
            }
            return _servicePostSettings.Get(join.ContentPostSettingsId);
        }
        #endregion

        /// <summary>
        /// Returns the the PostParent with the Parent entity set
        /// </summary>
        /// <param name="post"></param>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        public ContentPostParentEntity GetPostParent(ContentPostEntity post)
        {
            var postParent = _servicePostParents.Find(x => x.ContentPostId == post.Id)?.FirstOrDefault();
            return postParent;
        }
        /// <summary>
        /// Returns the the PostParent with the Parent entity set
        /// </summary>
        /// <param name="post"></param>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        public ContentPostParentEntity GetPostParentWithParent(ContentPostEntity post)
        {
            var postParent = _servicePostParents.Find(x => x.ContentPostId == post.Id)?.FirstOrDefault();
            if (postParent == null || postParent.ParentId == null) return null;
            postParent.Parent = _servicePosts.Get(postParent.ParentId.GetValueOrDefault());
            return postParent;
        }
        #endregion

        #region Links

        public async Task<int> CreateLink(ContentPostLinkEntity entity)
        {
            return await _servicePostLink.Create(entity);
        }
        public ContentPostLinkEntity GetLink(Guid postId)
        {
            return _servicePostLink.Find(x => x.ContentPostId == postId)?.FirstOrDefault();
        }
        #endregion

        #region
        /// <summary>
        /// Returns the channel if the post has a channel
        /// </summary>
        /// <param name="post"></param>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        public ContentChannelEntity GetPostChannel(ContentPostEntity post)
        {
            var channelPost = _servicePostChannel.Find(x => x.ContentPostId == post.Id)?.FirstOrDefault();
            if (channelPost == null) return null;
            return _serviceChannels.Get(channelPost.ChannelId);
        }
        #endregion

        #region Quotes
        #endregion

        #region Media
        public ContentPostMediaEntity CreateMediaLink(ContentPostEntity post, Guid mediaId)
        {
            return new ContentPostMediaEntity()
            {
                MediaBlobId = mediaId,
                ContentPostId = post.Id,
            };
        }
        #endregion
    }
}

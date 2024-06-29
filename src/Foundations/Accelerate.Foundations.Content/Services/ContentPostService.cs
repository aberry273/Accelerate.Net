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
using Newtonsoft.Json.Linq;
using static Accelerate.Foundations.Content.Constants.Defaults;
using Elastic.Clients.Elasticsearch.IndexManagement;
using static MassTransit.MessageHeaders;
using System.Threading.Channels;
using Accelerate.Foundations.Common.Services;

namespace Accelerate.Foundations.Content.Services
{
    public class ContentPostService : IContentPostService
    {
        readonly Bind<IContentActionsBus, IPublishEndpoint> _publishActionsEndpoint; 
        readonly Bind<IContentPostActivityBus, IPublishEndpoint> _publishActivityEndpoint;
        readonly Bind<IContentPostQuoteBus, IPublishEndpoint> _publishQuoteEndpoint;
        readonly Bind<IContentPostParentBus, IPublishEndpoint> _publishParentEndpoint;
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
            Bind<IContentActionsBus, IPublishEndpoint> publishActionsEndpoint,
            Bind<IContentPostActivityBus, IPublishEndpoint> publishActivityEndpoint,
            Bind<IContentPostQuoteBus, IPublishEndpoint> publishQuoteEndpoint,
            Bind<IContentPostParentBus, IPublishEndpoint> publishParentEndpoint,
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
            this._publishActivityEndpoint = publishActivityEndpoint;
            this._publishActionsEndpoint = publishActionsEndpoint;
            this._publishQuoteEndpoint = publishQuoteEndpoint;
            this._publishParentEndpoint = publishParentEndpoint;
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
        public async Task RunCreateActionsPipeline(ContentPostActionsEntity obj)
        {
            await _publishActionsEndpoint.Value.Publish(new CreateDataContract<ContentPostActionsEntity>()
            {
                Data = obj,
                UserId = obj.UserId
            });
        }
        public async Task RunUpdateActionsPipeline(ContentPostActionsEntity obj)
        {
            await _publishActionsEndpoint.Value.Publish(new UpdateDataContract<ContentPostActionsEntity>()
            {
                Data = obj,
                UserId = obj.UserId
            });
        }

        public async Task RunCreateQuotePipeline(ContentPostQuoteEntity obj)
        {
            await _publishQuoteEndpoint.Value.Publish(new CreateDataContract<ContentPostQuoteEntity>()
            {
                Data = obj,
                UserId = obj.UserId
            });
        }
        public async Task RunCreateParentPipeline(ContentPostParentEntity obj)
        {
            await _publishParentEndpoint.Value.Publish(new CreateDataContract<ContentPostParentEntity>()
            {
                Data = obj,
                UserId = obj.UserId
            });
        }

        public async Task RunCreateActivityPipeline(ContentPostActivityEntity obj)
        {
            await _publishActivityEndpoint.Value.Publish(new CreateDataContract<ContentPostActivityEntity>()
            {
                Data = obj,
                UserId = obj.UserId
            });
        }

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
        #region Post Summary
        public async Task<int> CreatePostSummary(ContentPostEntity post)
        {
            var entity = new ContentPostActionsSummaryEntity()
            {
                ContentPostId = post.Id,
            };
            return await _serviceActionsSummary.Create(entity);
        }
        public void UpdatePostSummary(ContentPostActionsSummaryEntity entity)
        {
            _serviceActionsSummary.UpdateNoSave(entity);
        }
        #endregion
        #region Channel
        public async Task<int> CreateChannelPost(ContentPostEntity post, Guid channelId)
        {
            var entity = new ContentPostChannelEntity()
            {
                ContentPostId = post.Id,
                ChannelId = channelId,
            };
            return await _servicePostChannel.Create(entity);
        }
        /// <summary>
        /// Returns the channel if the post has a channel
        /// </summary>
        /// <param name="post"></param>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        public ContentChannelEntity GetPostChannel(ContentPostEntity post)
        {
            var postChannel = _servicePostChannel.Find(x => x.ContentPostId == post.Id).FirstOrDefault();
            if (postChannel == null) return null;
            return _serviceChannels.Get(postChannel.ChannelId);
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
            //add activity that a child post was addeed to the parent
            await CreatePostActivity(parentId, ContentPostActivityTypes.Reply, post.Id.ToString());
            var result = await _servicePostParents.Create(entity);

            // No need to fetch the actual entity as we don't need it's ID
            await RunCreateParentPipeline(entity);

            return result;
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

        #region Actions 
        public async Task<Guid?> CreateOrUpdatePostAction(ContentPostActionsEntity obj)
        {
            Guid? result;
            var existingAction = _servicePostActions.Find(x => x.UserId == obj.UserId && x.ContentPostId == obj.ContentPostId, 0, 1).FirstOrDefault();
            bool? agree = obj.Agree, disagree = obj.Disagree, like = obj.Like;
            if (existingAction != null)
            {
                agree = existingAction.Agree == true && obj.Agree == false ? false : obj.Agree;
                disagree = existingAction.Disagree == true && obj.Disagree == false ? false : obj.Disagree;

                if (obj.Agree == true)
                {
                    disagree = false;
                }
                if (obj.Disagree == true)
                {
                    agree = false;
                }
                var count = await this.UpdatePostAction(existingAction, agree, disagree, like);
                result = count > 0 ? obj.Id : null;
            }
            else
            {
                result = await this.CreatePostAction(obj.ContentPostId, obj.UserId, agree, disagree, like);
            }
            // move to pipeline
            if (agree == true)
            {
                await this.CreatePostActivity(obj.ContentPostId, ContentPostActivityTypes.Vote, "agree");
            }
            if (disagree == true)
            {
                await this.CreatePostActivity(obj.ContentPostId, ContentPostActivityTypes.Vote, "disagree");
            }
            return result;
        }
        public async Task<Guid?> CreatePostAction(Guid postId, Guid userId, bool? agree, bool? disagree, bool? like)
        {
            var action = new ContentPostActionsEntity()
            {
                UserId = userId,
                ContentPostId = postId,
                Agree = agree,
                Disagree = disagree,
                Like = like,
            };
            var guid = await _servicePostActions.CreateWithGuid(action);

            var entity = _servicePostActions.Get(guid.GetValueOrDefault());

            await RunCreateActionsPipeline(entity);
            return guid;

        }
        public async Task<int> UpdatePostAction(ContentPostActionsEntity entity, bool? agree, bool? disagree, bool? like)
        {
            entity.Agree = agree;
            entity.Disagree = disagree;
            entity.Like = like;

            var count = await _servicePostActions.Update(entity);
             
            await RunUpdateActionsPipeline(entity);
            return count;
        }
        #endregion

        #region Activities
        private ContentPostActivityEntity CreateActivity(Guid postId, ContentPostActivityTypes type, string value)
        {
            return new ContentPostActivityEntity()
            {
                ContentPostId = postId,
                Type = type,
                Value = value,
            };
        }
        public async Task<Guid?> CreatePostActivity(Guid postId, ContentPostActivityTypes type, string value)
        {
            var action = CreateActivity(postId, type, value);
            var activityGuid = await _servicePostActivities.CreateWithGuid(action);
            if (activityGuid == null) return activityGuid;
            var activity = _servicePostActivities.Get(activityGuid.GetValueOrDefault());
            await RunCreateActivityPipeline(activity);
            return activityGuid.GetValueOrDefault();
        }
        #endregion

        #region Quotes

        public async Task ProcessMultiplePipelinesAsync(IEnumerable<ContentPostQuoteEntity> quotes)
        { 
            await Task.WhenAll(quotes.Select(q =>
            {
                try
                {
                    // No need to fetch the actual entity as we don't need it's ID
                    return RunCreateQuotePipeline(q);
                }
                catch (Exception e)
                {
                    return Task.FromException(e);
                }
            })); 
        } 

        public async Task<int> CreateQuotes(Guid postId, List<Guid> quoteIds)
        {
            var quotes = quoteIds.Select(x => CreateQuoteLink(postId, x));

            var quoteResults = await _servicePostQuotes.AddRange(quotes);

            await ProcessMultiplePipelinesAsync(quotes);

            //add activities each each quoted post that it was quoted
            var activityEntities = quotes.Select(x => CreateActivity(x.QuotedContentPostId, ContentPostActivityTypes.Quote, postId.ToString()));

            var activityResults = await _servicePostActivities.AddRange(activityEntities);


            return quoteResults;
        }

        public ContentPostQuoteEntity CreateQuoteLink(Guid postId, Guid quotedPostId)
        {
            // append current thread to path of new quote, or set as default path 
            return new ContentPostQuoteEntity()
            {
                QuotedContentPostId = quotedPostId,
                ContentPostId = postId,
                //Content = quote.Content,
                //Response = quote.Response
            };
        }


        
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

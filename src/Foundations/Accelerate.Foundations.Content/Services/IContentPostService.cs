﻿using Accelerate.Foundations.Content.Hydrators;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.EventPipelines.Models.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Content.Services
{
    public interface IContentPostService
    {
        Task<ContentPostEntity> CreatePost(
            ContentPostEntity obj,
            Guid? channelId,
            Guid? parentId,
            //List<Guid> parentIds,
            List<Guid> mentionUserIds,
            List<Guid> quoteIds,
            List<Guid> mediaIds,
            ContentPostSettingsEntity settings,
            ContentPostLinkEntity linkCard,
            ContentPostTaxonomyEntity taxonomy);

        Task<ContentPostEntity> CreateWithPipeline(ContentPostEntity obj);
        Task<ContentPostEntity> Create(ContentPostEntity obj);
        Task<ContentPostEntity> Update(Guid id, ContentPostEntity obj);
        Task<int> Delete([FromRoute] Guid id);

        Task RunCreatePipeline(ContentPostEntity obj, dynamic Params = null);
        Task RunUpdatePipeline(ContentPostEntity obj, dynamic Params = null);
        Task RunDeletePipeline(ContentPostEntity obj);

        ContentPostMediaEntity CreateMediaLink(Guid postId, Guid mediaId);

        Task<int> CreatePostSummary(ContentPostEntity post);
        void UpdatePostSummary(ContentPostActionsSummaryEntity entity);

        Task<int> CreateParentPost(ContentPostEntity post, Guid parentId);//, List<Guid> ancestorIds);
        ContentPostParentEntity GetPostParent(ContentPostEntity post);
        ContentPostParentEntity GetPostParentWithParent(ContentPostEntity post);
        List<ContentPostEntity> GetReplies(Guid postId);
        int GetReplyCount(Guid postId);

        Task<int> CreateLink(ContentPostLinkEntity entity);
        ContentPostLinkEntity GetLink(Guid postId);

        Task<int> CreateSettings(Guid postId, ContentPostSettingsEntity entity);
        ContentPostSettingsEntity GetSettings(Guid postId);

        Task<Guid?> CreateTaxonomy(Guid postId, ContentPostTaxonomyEntity entity);
        ContentPostTaxonomyEntity GetTaxonomy(Guid postId);


        Task<int> CreateChannelPost(ContentPostEntity post, Guid channelId);
        ContentChannelEntity GetPostChannel(ContentPostEntity post);

        Task<int> CreateQuotes(Guid postId, List<Guid> quoteIds);
         
        Task<int> CreateMentions(Guid postId, List<Guid> userIds);

        //actions
        Task<Guid?> CreateOrUpdatePostAction(ContentPostActionsEntity obj);
        Task<Guid?> CreatePostAction(Guid postId, Guid userId, bool? agree, bool? disagree, bool? like, string? reaction);
    }
}

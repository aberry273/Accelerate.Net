using Accelerate.Foundations.Content.Hydrators;
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
        Task<ContentPostEntity> CreateWithPipeline(ContentPostEntity obj);
        Task<ContentPostEntity> Create(ContentPostEntity obj);
        Task<ContentPostEntity> Update(Guid id, ContentPostEntity obj);
        Task<int> Delete([FromRoute] Guid id);

        Task RunCreatePipeline(ContentPostEntity obj);
        Task RunUpdatePipeline(ContentPostEntity obj);
        Task RunDeletePipeline(ContentPostEntity obj);

        ContentPostMediaEntity CreateMediaLink(ContentPostEntity post, Guid mediaId);

        ContentChannelEntity GetPostChannel(ContentPostEntity post);

        Task<int> CreateParentPost(ContentPostEntity post, Guid parentId, List<Guid> ancestorIds);
        ContentPostParentEntity GetPostParent(ContentPostEntity post);
        ContentPostParentEntity GetPostParentWithParent(ContentPostEntity post);
        List<ContentPostEntity> GetReplies(Guid postId);
        int GetReplyCount(Guid postId);

        Task<int> CreateLink(ContentPostLinkEntity entity);
        ContentPostLinkEntity GetLink(Guid postId);

        Task<int> CreateSettings(Guid postId, ContentPostSettingsEntity entity);
        ContentPostSettingsEntity GetSettings(Guid postId);
    }
}

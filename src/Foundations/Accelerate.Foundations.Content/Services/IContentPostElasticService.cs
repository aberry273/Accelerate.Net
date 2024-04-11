using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Content.Models.Data;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Clients.Elasticsearch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch.IndexManagement;

namespace Accelerate.Foundations.Content.Services
{
    public interface IContentPostElasticService
    {
        Task<List<ContentChannelDocument>> SearchChannels(RequestQuery Query);
        Task<List<ContentPostDocument>> SearchPosts(RequestQuery Query);
        Task<List<ContentPostReviewDocument>> SearchUserReviews(RequestQuery Query);
        QueryDescriptor<ContentPostDocument> BuildRepliesSearchQuery(string threadId);

        QueryDescriptor<ContentPostDocument> BuildSearchQuery(RequestQuery Query);
        Query CreateTerm(QueryFilter filter);
        Query[] GetQueries(RequestQuery request, ElasticCondition condition);
        Task<DeleteIndexResponse> DeleteIndex();
    }
}

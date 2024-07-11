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
using Accelerate.Foundations.Content.Models.Entities;

namespace Accelerate.Foundations.Content.Services
{
    public interface IContentPostElasticService
    {
        Task<List<ContentChannelDocument>> SearchChannels(RequestQuery Query);
        Task<ContentSearchResults> SearchUserPosts(Guid userId, int page = 0, int itemsPerPage = 10);
        Task<ContentSearchResults> SearchPosts(RequestQuery Query, string sortField = Foundations.Integrations.Elastic.Constants.Fields.CreatedOn, Elastic.Clients.Elasticsearch.SortOrder sortOrder = Elastic.Clients.Elasticsearch.SortOrder.Desc);
        Task<ContentSearchResults> SearchPost(RequestQuery Query, Guid postId, string sortField = Foundations.Integrations.Elastic.Constants.Fields.CreatedOn, Elastic.Clients.Elasticsearch.SortOrder sortOrder = Elastic.Clients.Elasticsearch.SortOrder.Desc);
        Task<ContentSearchResults> SearchPostReplies(RequestQuery Query, string sortField = Foundations.Integrations.Elastic.Constants.Fields.CreatedOn, Elastic.Clients.Elasticsearch.SortOrder sortOrder = Elastic.Clients.Elasticsearch.SortOrder.Desc);
        Task<ContentSearchResults> SearchRelatedPosts(ContentChannelDocument channel, RequestQuery query, int page = 0, int itemsPerPage = 10);
        Task<ContentSearchResults> SearchPostParents(RequestQuery Query, Guid postId, Guid userId);
        Task<List<ContentPostActionsDocument>> SearchUserActions(RequestQuery Query);
        Task<List<ContentPostActionsSummaryDocument>> SearchPostActionSummaries(RequestQuery Query, List<string> ids);
        QueryDescriptor<ContentPostDocument> BuildRepliesSearchQuery(string threadId);
        QueryDescriptor<ContentPostDocument> BuildSearchRepliesQuery(RequestQuery Query);
        QueryDescriptor<ContentPostDocument> BuildAscendantsSearchQuery(ContentPostDocument item);
        QueryDescriptor<ContentPostDocument> BuildSearchQuery(RequestQuery Query);
        Query? CreateTerm(QueryFilter filter);
        Query[] GetQueries(RequestQuery request, ElasticCondition condition);
        Task<DeleteIndexResponse> DeleteIndex();
        RequestQuery<ContentPostDocument> CreateUserPostQuery(Guid userId); 
        RequestQuery<ContentPostDocument> CreateAggregateQuery(Guid? threadId, List<QueryFilter> filters, List<string> fields);
        RequestQuery<ContentPostDocument> CreateChannelAggregateQuery(Guid channelId);
        RequestQuery<ContentPostDocument> CreateThreadAggregateQuery(List<QueryFilter> filters = null);

    }
}

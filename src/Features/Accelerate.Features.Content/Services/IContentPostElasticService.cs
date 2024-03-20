using Accelerate.Features.Content.Models.Data;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Content.Models;
using Elastic.Clients.Elasticsearch;

namespace Accelerate.Features.Content.Services
{
    public interface IContentPostElasticService
    {
        Task<IndexResponse> Index(ContentPostEntity entity);
        Task<SearchResponse<ContentPostEntity>> Find(RequestQuery<ContentPostEntity> query);
    }
}

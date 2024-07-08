using Accelerate.Foundations.Content.Models.Data;
using Elastic.Clients.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Content.Services
{
    public interface IContentActivityElasticService
    {
        Task<SearchResponse<ContentPostActivityDocument>> SearchUserActivities(Guid userId, int page = 0, int itemsPerPage = 10);
    }
}

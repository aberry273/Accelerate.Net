using Accelerate.Foundations.Users.Models;
using Accelerate.Foundations.Users.Models.Data;
using Accelerate.Foundations.Common.Models;
using Elastic.Clients.Elasticsearch.QueryDsl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Users.Services
{
    public interface IUsersUserSearchService
    {
        Task<UsersSearchResults> SearchUsers(RequestQuery Query);
        Task<UsersSearchResults> SearchUsers(RequestQuery Query, List<string> userIds);
    }
}

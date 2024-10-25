﻿using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Account.Models.Data;
using Accelerate.Foundations.Common.Models;
using Elastic.Clients.Elasticsearch.QueryDsl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Account.Services
{
    public interface IAccountUserSearchService
    {
        Task<AccountSearchResults> SearchUsers(RequestQuery Query);
        Task<AccountSearchResults> SearchUsers(RequestQuery Query, List<string> userIds);
    }
}

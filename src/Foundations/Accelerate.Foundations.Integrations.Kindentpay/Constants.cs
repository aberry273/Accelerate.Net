using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Integrations.Elastic
{
    public struct Constants
    {
        public struct Search
        {
            public const int MaxQueryable = 100;
            public const int DefaultPerPage = 10;
        }
        public struct Fields
        {
            public const string Id = "id";
            public const string CreatedOn = "createdOn";
            public const string UpdatedOn = "updatedOn";
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Features.Media
{
    public struct Constants
    {
        public struct Fields
        {
            public const string FileName = "fileName";
            public const string UserId = "userId";
            public const string Id = "id";
            public const string Tags = "tags";
            public const string Status = "status";
        }
        public struct Websockets
        {
            public const string MediaBlobsHubName = "MediaBlobs";

        }
    }
}
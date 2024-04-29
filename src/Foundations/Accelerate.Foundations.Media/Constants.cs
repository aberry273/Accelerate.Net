using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Media
{
    public struct Constants
    {
        public struct Paths
        {
            public const string MediaFile = "media/files";
        }
        public struct Config
        {
            public const string ConfigName = "MediaConfiguration";
            public const string MediaIndexName = "MediaIndexName";

            public const string LocalDatabaseKey = "LocalMediaContext";
            public const string DatabaseKey = "MediaContext";
        }
        public struct Fields
        {
        }
    }
}
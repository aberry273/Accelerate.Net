using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Services
{
    public interface IRssReaderService
    {
        public SyndicationFeed Read(string url);
    }
}

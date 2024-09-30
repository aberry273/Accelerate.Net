using System.ServiceModel.Syndication;
using System.Xml;

namespace Accelerate.Foundations.Common.Services
{
    public class RssReaderService : IRssReaderService
    {
        public RssReaderService()
        {
        }
        public SyndicationFeed Read(string url)
        {
            using var reader = XmlReader.Create(url);
            return SyndicationFeed.Load(reader);
        }
    }
}

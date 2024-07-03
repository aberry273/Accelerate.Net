using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Content.Models.Data
{
    public class ContentConfiguration
    {
        public string PostIndexName { get; set; }
        public string ChannelIndexName { get; set; }
        public string ActionsIndexName { get; set; }
        public string QuotesIndexName { get; set; }
        public string MediaIndexName { get; set; }
        public string ActionsSummaryIndexName { get; set; }
    }
}

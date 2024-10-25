﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Operations.Models.Data
{
    public class OperationsConfiguration
    {
        public string PostIndexName { get; set; }
        public string ChannelIndexName { get; set; }
        public string FeedIndexName { get; set; }
        public string ChatIndexName { get; set; }
        public string ListIndexName { get; set; }
        public string ActivitiesIndexName { get; set; }
        public string ActionsIndexName { get; set; }
        public string QuotesIndexName { get; set; }
        public string MediaIndexName { get; set; }
        public string ActionsSummaryIndexName { get; set; }
    }
}
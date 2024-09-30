using Accelerate.Foundations.Common.Helpers;
using Accelerate.Foundations.Common.Models.Data;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Communication.Models;
using Accelerate.Foundations.Communication.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static Accelerate.Foundations.Common.Helpers.ObjectHelper;

namespace Accelerate.Foundations.Operations.Actions
{
    public class RssReaderActionData
    {
        public string Url { get; set; }
    }
    public class RssReaderActionSettings
    {
        public string Url { get; set; }
    }
    public class RssReaderAction : BaseAction<RssReaderActionSettings>
    {
        IRssReaderService _rssReaderService;
        private EmailConfiguration _emailConfig;
        private EmailSender _sender;
        private string _simpleEmailTemplate;
        private string _listingEmailTemplate;
        private string _contactEmailTemplate;
        public RssReaderAction(string settings) : base(settings)
        {
            this.Init();
            this._rssReaderService = new RssReaderService();
        }
        public RssReaderAction(RssReaderActionSettings settings) : base(settings)
        {
            this.Init();
            this._rssReaderService = new RssReaderService();
        }
        private void Init()
        {
            this.Name = "RssReader";
            this.Data = "{}";

            //see https://colorlib.com/etc/email-template/6/index.html
            //_listingEmailTemplate = this.LoadTemplate("email/4/index.inline.html");
            //_simpleEmailTemplate = this.LoadTemplate("email/5/index.inline.html");
            //_contactEmailTemplate = this.LoadTemplate("email/6/index.inline.html");
        }

        public override async Task<OperationResponse<object>> RunAsync(string Data)
        {
            var data = JsonConvert.DeserializeObject<RssReaderActionData>(Data);
            return await this.RunAsync(data);
        }

        public async Task<OperationResponse<object>> RunAsync(RssReaderActionData data)
        {
            var response = new OperationResponse<object>()
            {
                Data = "RSS Reader Run",
                Success = true
            };

            // var url = "https://khalidabuhakmeh.com/feed.xml";
            var docs = _rssReaderService.Read(data.Url);
            response.Data = $"RSS Reader Run: {docs.Items.Count()} new documents";
            return response;
        }
    }
}

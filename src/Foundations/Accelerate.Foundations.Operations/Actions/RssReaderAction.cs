using Accelerate.Foundations.Common.Helpers;
using Accelerate.Foundations.Common.Models.Data;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Communication.Models;
using Accelerate.Foundations.Communication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Accelerate.Foundations.Common.Helpers.ObjectHelper;

namespace Accelerate.Foundations.Operations.Actions
{
    public class RssContentPostFormatData
    {
        public List<string>? items { get; set; }
        public string? style { get; set; }
        public string? text { get; set; }
    }
    public class RssContentPostFormatItem
    {
        public string id { get; set; }
        public string type { get; set; }
        public RssContentPostFormatData data { get; set; }
    }
    public class RssContentPostRequestObject
    {
        // Core properties
        public Guid UserId { get; set; }
        public int Status { get; set; } = 1;
        public string? ExternalId { get; set; }
        public string? Text { get; set; }
        public string? Formats { get; set; }
        [NotMapped]
        public List<RssContentPostFormatItem>? FormatItems
        {
            get
            {
                if (string.IsNullOrEmpty(Formats)) return null;
                return System.Text.Json.JsonSerializer.Deserialize<List<RssContentPostFormatItem>>(Formats);
            }
            set
            {
                Formats = System.Text.Json.JsonSerializer.Serialize(value);
            }
        }
        public List<string>? Tags { get; set; }
        public string? Category { get; set; }
        // Other properties
        public string? LinkUrl { get; set; }
        public Guid? ChannelId { get; set; }
        public Guid? ChatId { get; set; }
        public Guid? ListId { get; set; }
        public Guid? ParentId { get; set; }
        public List<Guid>? QuotedIds { get; set; }
        //Settings
        public int? CharLimit { get; set; }
        public int? WordLimit { get; set; }
        public int? VideoLimit { get; set; }
        public int? ImageLimit { get; set; }
        public int? QuoteLimit { get; set; }
        public string? Access { get; set; }
    }
    public class RssReaderActionData
    {
        public string Url { get; set; }
    }
    public class RssReaderActionSettings
    {
        public string Url { get; set; }
        public string BearerToken { get; set; }
        public string BasicUsername { get; set; }
        public string BasicPassword { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public PostbackActionType Type { get; set; }
        public bool Async { get; set; }
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
            var items = docs.Items.Select(CreateItemRequestObject).ToList();
            int counter = 0;
            for(var i = 0; i < items.Count; i++)
            {
                await CreateContentPostItem(items[i]);
                counter++;
            }
            response.Data = $"RSS Reader Run: Created {counter} new documents";
            return response;
        }

        #region Create Posts by API


        private RssContentPostRequestObject CreateItemRequestObject(SyndicationItem item)
        {
            var model = new RssContentPostRequestObject();
            model.ExternalId = item.Id;
            var link = item.Links.FirstOrDefault();
            model.FormatItems = new List<RssContentPostFormatItem>()
            {
                /*
                new RssContentPostFormatItem()
                {
                    id = $"{item.Id}-title",
                    type = "paragraph",
                    data = new RssContentPostFormatData()
                    {
                        text = $"<b>{item.Title.Text}</b>"
                    }
                },
                new RssContentPostFormatItem()
                {
                    id = $"{item.Id}-description",
                    type = "paragraph",
                    data = new RssContentPostFormatData()
                    {
                        text = item.Summary.Text
                    }
                },
                new RssContentPostFormatItem()
                {
                    id = $"{item.Id}-description",
                    type = "paragraph",
                    data = new RssContentPostFormatData()
                    {
                        text = $"<a target=\"_blank\" class=\"underline font-semibold\t\" href=\"{link?.Uri.ToString()}\">Read more</a>"
                    }
                },
                */
            };
            model.LinkUrl = link?.Uri.ToString();
            model.Tags = item.Categories.Select(x => x.Name).ToList();
            model.Category = "News";
            //model.Category = link.Uri.Host.Split('.')[1];
            model.ChannelId = Foundations.Common.Constants.Global.ChannelNewsGuid;
            // System user
            model.UserId = Foundations.Common.Constants.Global.GlobalAdminContent;
            return model;
        }
        private static string _postUrl = "https://localhost:7220/api/contentpost/json";
        public async Task<OperationResponse<object>> CreateContentPostItem(RssContentPostRequestObject item)
        {
            var result = new OperationResponse<object>()
            {
                Success = true
            };
            try
            {
                using (var httpClient = new HttpClient())
                {
                    this.SetHeaders(httpClient);
                    var itemStr = Foundations.Common.Helpers.JsonSerializerHelper.SerializeObject(item);
                    StringContent httpContent = new StringContent(itemStr, System.Text.Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync(_postUrl, httpContent))
                    {
                        result.Data = await response.Content.ReadAsStringAsync();
                        return result;
                    } 
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.InnerException.ToString();
                return result;
            }
        }

        private void SetHeaders(HttpClient httpClient)
        {
            //Set auth
            if (!string.IsNullOrEmpty(this.Settings.BearerToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Settings.BearerToken);
            }
            else if (!string.IsNullOrEmpty(this.Settings.BasicUsername) && !string.IsNullOrEmpty(this.Settings.BasicPassword))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes($"{this.Settings.BasicUsername}:{this.Settings.BasicPassword}")));
            }

            // Set headers
            if (this.Settings.Headers != null && this.Settings.Headers.Any())
            {
                for (var i = 0; i < this.Settings.Headers.Count; i++)
                {
                    var hdr = this.Settings.Headers.ElementAt(i);
                    if (hdr.Key.ToLower() == "content-type")
                    {
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(hdr.Value));
                    }
                    else
                    {
                        httpClient.DefaultRequestHeaders.Add(hdr.Key, hdr.Value);
                    }
                }
            }
            else
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }
        #endregion
    }
}

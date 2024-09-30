
using Accelerate.Foundations.Common.Models.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Operations.Actions
{

    public class BulkEmailActionSettings
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public EmailActionType Type { get; set; }
        public IEnumerable<string> Recipients { get; set; }
    }
    public class BulkEmailAction : BaseAction<BulkEmailActionSettings>
    {
        public BulkEmailAction(string settings) : base(settings)
        {
            this.Name = nameof(BulkEmailAction);
            this.Data = "{}";
        }
        public BulkEmailAction(BulkEmailActionSettings settings) : base(settings)
        {
            this.Name = nameof(BulkEmailAction);
            this.Data = "{}";
        }
        public override async Task<OperationResponse<object>> RunAsync(string Data)
        {
            var bulkData = JsonConvert.DeserializeObject<EmailActionData>(Data);
            return await RunAsync(bulkData);
        }
        public async Task<OperationResponse<object>> RunAsync(EmailActionData data)
        {
            int cnt = 0;
            for (var i = 0; i < this.Settings.Recipients.Count(); i++)
            {
                try
                {
                    var recipient = this.Settings.Recipients.ElementAt(i);
                    var settings = new EmailActionSettings()
                    {
                        Email = recipient,
                        Subject = this.Settings.Subject,
                        Type = this.Settings.Type,
                    };
                    var emailAction = new EmailAction(settings);
                    await emailAction.RunAsync(data);
                    cnt++;
                }
                catch (Exception ex)
                {

                }
            }
            return new OperationResponse<object>()
            {
                Success = true,
                Data = $"{cnt} of {this.Settings.Recipients.Count()} processed."
            };
        }
    }
}

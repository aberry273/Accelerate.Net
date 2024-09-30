using Accelerate.Foundations.Common.Helpers;
using Accelerate.Foundations.Common.Models.Data;
using Accelerate.Foundations.Communication.Models;
using Accelerate.Foundations.Communication.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Accelerate.Foundations.Common.Helpers.ObjectHelper;

namespace Accelerate.Foundations.Operations.Actions
{
    public enum EmailActionType
    {
        Simple, Listing, Contact
    }
    public class EmailActionData
    {
        public string Title { get; set; }
        /*
        public Rocketbase.Shared.Foundation.Components.Models.Data.AclDataModel Background { get; set; }
        public Rocketbase.Shared.Foundation.Components.Models.Data.NavigationLinks Links { get; set; }
        public JobListingModel Listing { get; set; }
        public Rocketbase.Shared.Foundation.Components.Models.Data.AclDataModel About { get; set; }
        public Rocketbase.Shared.Foundation.Components.Models.Data.AclDataModel Contact { get; set; }
        public Rocketbase.Shared.Foundation.Components.Models.Data.AclDataModel Copyright { get; set; }
        public Rocketbase.Shared.Foundation.Components.Models.Data.NavigationLink Unsubscribe { get; set; }
        */
    }
    public class EmailActionSettings
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public EmailActionType Type { get; set; }
    }
    public class EmailAction : BaseAction<EmailActionSettings>
    {
        private EmailConfiguration _emailConfig;
        private EmailSender _sender;
        private string _simpleEmailTemplate;
        private string _listingEmailTemplate;
        private string _contactEmailTemplate;
        public EmailAction(string settings) : base(settings)
        {
            this.Init();
        }
        public EmailAction(EmailActionSettings settings) : base(settings)
        {
            this.Init();
        }
        private void Init()
        {
            this.Name = "Email";
            this.Data = "{}";

            //see https://colorlib.com/etc/email-template/6/index.html
            //_listingEmailTemplate = this.LoadTemplate("email/4/index.inline.html");
            //_simpleEmailTemplate = this.LoadTemplate("email/5/index.inline.html");
            //_contactEmailTemplate = this.LoadTemplate("email/6/index.inline.html");
        }

        public override async Task<OperationResponse<object>> RunAsync(string Data)
        {
            var data = JsonConvert.DeserializeObject<EmailActionData>(Data);
            return await this.RunAsync(data);
        }

        public async Task<OperationResponse<object>> RunAsync(EmailActionData data)
        {
            var response = new OperationResponse<object>()
            {
                Data = "Email Sent",
                Success = true
            };
            switch (this.Settings.Type)
            {
                case EmailActionType.Simple:
                    {
                        //await SingletonServices.MessageServices.SendEmailAsync(BuildEmailMessage(_simpleEmailTemplate, data));
                        return response;
                    }
                case EmailActionType.Listing:
                    {
                        //await SingletonServices.MessageServices.SendEmailAsync(BuildEmailMessage(_listingEmailTemplate, data));
                        return response;
                    }
                case EmailActionType.Contact:
                    {
                        //await SingletonServices.MessageServices.SendEmailAsync(BuildEmailMessage(_contactEmailTemplate, data));
                        return response;
                    }
                default:
                    {
                        response.Success = false;
                        response.Message = "No matching email template found";
                        return response;
                    }
            }
        }
        private string LoadTemplate(string templatePath)
        {
            var simplePath = Path.Combine("Templates", templatePath);
            using (StreamReader reader = new StreamReader(simplePath))
            {
                return reader.ReadToEnd();
            }
        }
        private EmailMessage BuildEmailMessage(string template, EmailActionData data)
        {
            //var tokens = data.GetPropertiesDeepRecursive("", new List<string>() { "Accelerator.Foundations.Operations.Actions" }, "$", "$");
            var objectConvertInto = new ObjectConvertInfo
            {
                Base = null,
                Prefix = "$",
                Postfix = "$",
                ConvertObject = data,
                IgnoreProperties = new List<string> { "IgnorePropertyA" },
                IgnoreTypes = new List<Type> { typeof(Array), typeof(IntPtr), typeof(Delegate), typeof(Type) },
                MaxDeep = 4
            };
            var tokens = ObjectHelper.ConvertObjectToDictionary(objectConvertInto);

            var linkItemsHtml = this.GetLinksHtml(data);
            var listingItemsHtml = this.GetListingHtml(data);

            var str = template
                .Replace("$title$", data.Title)
                .Replace("$links$", linkItemsHtml)
                .Replace("$listing$", listingItemsHtml)
                //.Replace("$unsubscribe.href$", data.Unsubscribe.Href)
                //.Replace("$unsubscribe.text$", data.Unsubscribe.Text)
                ;

            str = StringReplacementHelper.ReplaceTokens(str, tokens);

            var model = new EmailMessage(new List<string>() { Settings.Email }, Settings.Subject, str);

            return model;
        }

        private string GetLinksHtml(EmailActionData data)
        {
            var linkItemsHtml = "";
            /*
            for (var i = 0; i < data.Links.Items.Count; i++)
            {
                var link = data.Links.Items.ElementAt(i);
                linkItemsHtml += $"<li style=\"-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;list-style: none;display: inline-block;margin-left: 5px;font-size: 12px;font-weight: 700;text-transform: uppercase;\"><a href=\"{link.Href}\" style=\"-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;text-decoration: none;color: rgba(0,0,0,.6);\">{link.Text}</a></li>";
            }
            */
            return linkItemsHtml;
        }
        private string GetListingHtml(EmailActionData data)
        {
            var listingItemTemplateHtml = @"
	              <td valign=""top"" width=""50%"" style=""padding-top: 20px; padding-right: 10px;"">
	                <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"">
	                  <tr>
	                    <td>
	                      <img src=""{0}"" alt="""" style=""width: 100%; max-width: 300px; height: auto; margin: auto; display: block;"">
	                    </td>
	                  </tr>
	                  <tr>
	                    <td class=""text-services"" style=""text-align: left;"">
                        <p style=""font-family:'Nunito Sans',sans-serif;color:#000000;margin:0;margin-bottom:0""><b>{1}</b></p>
                        {2}
	                    	<h3 style=""font-family:'Nunito Sans',sans-serif;color:#000000;margin-top:0;margin-bottom:0"">{3}</h3>
	                     	{4}
	                     	{5}
	                     	{6}
	                     	{7}
	                     	{8}
	                    </td>
	                  </tr>
	                </table>
	              </td>";
            var listingItemPrimaryHtml = @"<p><a href=""{0}"" style=""font-family:'Nunito Sans',sans-serif;color:#000000;margin-top:0;margin-bottom:0"">{1}</a></p>";
            var listingItemsHtml = "<tr>";
            int cntr = 0;
                /*
            for (var i = 0; i < data.Listing.Items.Count(); i++)
            {
                var link = data.Listing.Items.ElementAt(i);
                var tagsHtml = GetTagsHtml(link.Tags);
                var siteHtml = GetSiteHtml(link.Url);
                var addressHtml = GetAddressHtml(link.Address);
                var emailHtml = GetEmailHtml(link.Email);
                var phoneHtml = GetPhoneHtml(link.Phone);
                var ctaHtml = link.Url != null ? String.Format(listingItemPrimaryHtml, Uri.EscapeUriString(link.Url), "Read more") : string.Empty;
                listingItemsHtml += String.Format(listingItemTemplateHtml, link.Image, link.Category, tagsHtml, link.Title, siteHtml, addressHtml, emailHtml, phoneHtml, ctaHtml);
                cntr++;
                if (cntr == 2)
                {
                    listingItemsHtml += "</tr><tr>";
                    cntr = 0;
                }
            }
                */
            listingItemsHtml += "</tr>";
            return listingItemsHtml;
        }
        private string GetTagsHtml(List<string> tags)
        {
            if (tags == null) return string.Empty;
            var tagsHtml = tags.Select(x => $"<span style=\"text-decoration:none;color:#ffffff;padding:1px 5px;font-size:11px;display:inline-block;border-radius:30px;background:#448ef6;\">{x}</span>");
            return $"<p style=\"margin:0px;\">{string.Join(' ', tagsHtml)}</p>";
        }

        private string GetSiteHtml(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;
            return $"<p style=\"margin:0px;\"><img src=\"https://trady180.blob.core.windows.net/uploads/2022-06-17/20220617T004615962.png\" alt=\"\" style=\"width: 100%; max-width: 15px; height: auto;\"> {url}</p>";
        }
        private string GetAddressHtml(string address)
        {
            if (string.IsNullOrEmpty(address)) return string.Empty;
            return $"<p style=\"margin:0px;\"><img src=\"https://trady180.blob.core.windows.net/uploads/2022-06-17/20220617T004620411.png\" alt=\"\" style=\"width: 100%; max-width: 15px; height: auto;\"> {address}</p>";
        }
        private string GetEmailHtml(string email)
        {
            if (string.IsNullOrEmpty(email)) return string.Empty;
            return $"<p style=\"margin:0px;\"><img src=\"https://trady180.blob.core.windows.net/uploads/2022-06-17/20220617T004624050.png\" alt=\"\" style=\"width: 100%; max-width: 15px; height: auto;\"> {email}</p>";
        }
        private string GetPhoneHtml(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return string.Empty;
            return $"<p style=\"margin:0px;\"><img src=\"https://trady180.blob.core.windows.net/uploads/2022-06-17/20220617T004628999.png\" alt=\"\" style=\"width: 100%; max-width: 15px; height: auto;\"> {phone}</p>";
        }

    }
}

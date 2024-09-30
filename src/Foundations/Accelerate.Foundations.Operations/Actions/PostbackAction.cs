
using Accelerate.Foundations.Common.Models.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Operations.Actions
{
    public enum PostbackActionType
    {
        GET, POST, PUT, DELETE, PATCH
    }
    public class PostbackActionSettings
    {
        public string Url { get; set; }
        public string BearerToken { get; set; }
        public string BasicUsername { get; set; }
        public string BasicPassword { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public PostbackActionType Type { get; set; }
        public bool Async { get; set; }
    }
    public class PostbackAction : BaseAction<PostbackActionSettings>
    {
        public PostbackAction(string settings) : base(settings)
        {
            this.Name = "Postback";
            this.Data = "{}";
        }

        public override async Task<OperationResponse<object>> RunAsync(string data)
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
                    switch (this.Settings.Type)
                    {
                        case PostbackActionType.GET:
                            {
                                using (var response = await httpClient.GetAsync(this.Settings.Url))
                                {
                                    result.Data = await response.Content.ReadAsStringAsync();
                                    return result;
                                }
                            }
                        case PostbackActionType.POST:
                            {
                                StringContent httpContent = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
                                using (var response = await httpClient.PostAsync(this.Settings.Url, httpContent))
                                {
                                    result.Data = await response.Content.ReadAsStringAsync();
                                    return result;
                                }
                            }
                        case PostbackActionType.PUT:
                            {
                                StringContent httpContent = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
                                using (var response = await httpClient.PutAsync(this.Settings.Url, httpContent))
                                {
                                    result.Data = await response.Content.ReadAsStringAsync();
                                    return result;
                                }
                            }
                        case PostbackActionType.DELETE:
                            {
                                using (var response = await httpClient.DeleteAsync(this.Settings.Url))
                                {
                                    result.Data = await response.Content.ReadAsStringAsync();
                                    return result;
                                }
                            }
                        case PostbackActionType.PATCH:
                            {
                                StringContent httpContent = new StringContent(data, System.Text.Encoding.UTF8, "application/json");
                                using (var response = await httpClient.PatchAsync(this.Settings.Url, httpContent))
                                {
                                    result.Data = await response.Content.ReadAsStringAsync();
                                    return result;
                                }
                            }
                        default:
                            {
                                using (var response = await httpClient.GetAsync(this.Settings.Url))
                                {
                                    result.Data = await response.Content.ReadAsStringAsync();
                                    return result;
                                }
                            }
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
        }
    }
}

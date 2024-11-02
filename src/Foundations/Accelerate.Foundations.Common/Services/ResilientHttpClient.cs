using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Services
{
    public class ResilientHttpClient : IResilientHttpClient
    {
        private readonly HttpClient _httpClient;
        public ResilientHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public void InitializeClient(string url)
        {
            _httpClient.BaseAddress = new Uri(url);

            // using Microsoft.Net.Http.Headers;
            // The GitHub API requires two headers.
            _httpClient.DefaultRequestHeaders.Add(
                HeaderNames.Accept, "application/vnd.github.v3+json");
            _httpClient.DefaultRequestHeaders.Add(
                HeaderNames.UserAgent, "HttpRequestsSample");
        }
        public async Task<IEnumerable<object>?> GetDocuments(string route) =>
        await _httpClient.GetFromJsonAsync<IEnumerable<object>>(route);
    }
}

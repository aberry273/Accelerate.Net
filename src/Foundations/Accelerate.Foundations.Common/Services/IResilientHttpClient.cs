using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Services
{
    public interface IResilientHttpClient
    {
        void InitializeClient(string url);
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
        Task<IEnumerable<object>?> GetDocuments(string route);
    }
}

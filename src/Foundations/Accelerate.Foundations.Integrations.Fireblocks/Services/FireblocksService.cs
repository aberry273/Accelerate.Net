using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using System.Text;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Integrations.Fireblocks.Models;

namespace Accelerate.Foundations.Integrations.Fireblocks.Services
{
    public class FireblocksService : IFireblocksService
    {
        private readonly string baseUrl = "https://api.fireblocks.io";
        FireblocksConfiguration _config;
        IResilientHttpClient _resilientHttpClient;
        private readonly RSA privateKey;

        public FireblocksService(IOptions<FireblocksConfiguration> options, IResilientHttpClient resilientHttpClient)
        {
            _config = options.Value;
            byte[] keyBytes = Convert.FromBase64String(_config.ApiSecret
                .Replace("-----BEGIN PRIVATE KEY-----", "")
                .Replace("-----END PRIVATE KEY-----", "")
                .Replace("\n", ""));

            var rsa = RSA.Create();
            rsa.ImportPkcs8PrivateKey(keyBytes, out _);

            privateKey = rsa;
        }
        public async Task<Newtonsoft.Json.Linq.JObject> Get(string path)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}{path}");
            req.Headers.Add("X-API-Key", _config.ApiKey);
            req.Headers.Add("Authorization", $"Bearer {SignJwt(path)}");

            using (var resp = await _resilientHttpClient.SendAsync(req))
            {
                var body = await resp.Content.ReadAsStringAsync();
                return Newtonsoft.Json.Linq.JObject.Parse(body);
            }
        }

        public async Task<Newtonsoft.Json.Linq.JObject> Post(string path, Newtonsoft.Json.Linq.JObject data)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}{path}");
            req.Headers.Add("X-API-Key", _config.ApiKey);
            req.Headers.Add("Authorization", $"Bearer {SignJwt(path, data.ToString())}");
            req.Content = new StringContent(data.ToString(), Encoding.UTF8, "application/json");

            using (var resp = await _resilientHttpClient.SendAsync(req))
            {
                var body = await resp.Content.ReadAsStringAsync();
                return Newtonsoft.Json.Linq.JObject.Parse(body);
            }
        }

        private string SignJwt(string path)
        {
            return SignJwt(path, string.Empty);
        }

        private string SignJwt(string path, string dataJSONString)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] dataHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(dataJSONString));
                string bodyHash = BitConverter.ToString(dataHash).Replace("-", "").ToLower();

                var now = DateTime.UtcNow;
                var expires = now.AddSeconds(55);

                var jwtClaims = new
                {
                    sub = _config.ApiKey,
                    iat = new DateTimeOffset(now).ToUnixTimeSeconds(),
                    exp = new DateTimeOffset(expires).ToUnixTimeSeconds(),
                    nonce = Guid.NewGuid().ToString(),
                    uri = path,
                    bodyHash
                };

                var jwtHeader = new Dictionary<string, object>
                {
                    { "alg", "RS256" },
                    { "typ", "JWT" }
                };

                var privateKeyParameters = privateKey.ExportParameters(true);
                var rsaParams = new RSAParameters
                {
                    Modulus = privateKeyParameters.Modulus,
                    Exponent = privateKeyParameters.Exponent,
                    D = privateKeyParameters.D,
                    P = privateKeyParameters.P,
                    Q = privateKeyParameters.Q,
                    DP = privateKeyParameters.DP,
                    DQ = privateKeyParameters.DQ,
                    InverseQ = privateKeyParameters.InverseQ
                };

                using (RSA rsa = RSA.Create())
                {
                    rsa.ImportParameters(rsaParams);

                    var jwt = Jose.JWT.Encode(jwtClaims, rsa, Jose.JwsAlgorithm.RS256, extraHeaders: jwtHeader);
                    return jwt;
                }
            }
        }
    }
}

using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Integrations.AzureSecrets.Services
{
    public class AzureKeyVaultService : IAzureKeyVaultService
    {
        SecretClient _client;
        public AzureKeyVaultService()
        {
            string keyVaultName = Environment.GetEnvironmentVariable("KEY_VAULT_NAME");
            var kvUri = "https://" + keyVaultName + ".vault.azure.net";

            _client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
        }

        public async Task<Azure.Response<KeyVaultSecret>> SetSecret(string secret, string value)
        {
            return await _client.SetSecretAsync(secret, value);
        }
        public async Task<Azure.Response<KeyVaultSecret>> GetSecret(string secret)
        {
            return await _client.GetSecretAsync(secret);
        }
        public async Task<DeleteSecretOperation> DeleteSecret(string secret)
        {
            return await _client.StartDeleteSecretAsync(secret);
        }
    }
}

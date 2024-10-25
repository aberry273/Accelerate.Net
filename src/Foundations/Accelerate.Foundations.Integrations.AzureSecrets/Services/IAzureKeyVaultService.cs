using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Integrations.AzureSecrets.Services
{
    interface IAzureKeyVaultService
    {
        Task<Azure.Response<KeyVaultSecret>> SetSecret(string secret, string value);
        Task<Azure.Response<KeyVaultSecret>> GetSecret(string secret);
        Task<DeleteSecretOperation> DeleteSecret(string secret);
    }
}

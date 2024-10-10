using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Account
{
    public struct Constants
    {
        public struct Domains
        {
            public const string Deleted = "Deleted";
            public const string Deactivated = "Deactivated";
            public const string Public = "Public";
            public const string System = "System";
            public const string Internal = "Internal";
        }
        public struct Fields
        {
            public const string Domain = "domain";
            public const string Username = "username";
            public const string Keyword = "keyword";
        }
        public struct Config
        {
            public const string ConfigName = "AccountConfiguration";
            public const string UserIndexName = "UserIndexName";

            public const string LocalDatabaseKey = "LocalAccountContext";
            public const string DatabaseKey = "AccountContext";

            public const string OAuthConfigurationName = "OAuthConfiguration";
            public const string FacebookAppIdKey = "FacebookAppId";
            public const string FacebookAppSecretKey = "FacebookAppSecret";
            public const string FacebookRedirectUri = "FacebookRedirectUri";
            public const string GoogleAppIdKey = "GoogleAppId";
            public const string GoogleAppSecretKey = "GoogleAppSecret";
            public const string GoogleRedirectUri = "GoogleRedirectUri";
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Users
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
            public const string ConfigName = "UsersConfiguration";
            public const string UserIndexName = "UserIndexName";
            public const string EnableOAuth = "EnableOAuth";

            public const string LocalDatabaseKey = "LocalUsersContext";
            public const string DatabaseKey = "UsersContext";

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Features.Account
{
    public struct Constants
    {
        public struct Claims
        {
            public const string Email = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
            public const string Givenname = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
            public const string Surname = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname";
            public const string Name = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
        }
        public struct Filters
        {
            public struct Media
            {
                public const string Type = "Type";
                public const string Tags = "Tags";
            }
            public struct Posts
            {
                public const string Actions = "Actions";
                public const string Threads = "Threads";
                public const string Quotes = "Quotes";
                public const string Tags = "Tags";
                public const string Status = "Status";
                public const string Content = "Content";
                public const string Sort = "Sort";
            }
        }
    }
}
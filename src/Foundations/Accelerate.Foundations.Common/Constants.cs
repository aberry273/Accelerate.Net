using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common
{
    public struct Constants
    {
        public struct Paths
        {
            public const string LoggedInPath = "/Channels";
            public const string LoggedInLabel = "Channels";
            public const string ProfilePath = "/Account/Profile";
            public const string ProfileLabel = "Profile";
            public const string LogoutPath = "/Account/Logout";
            public const string LogoutLabel = "Logout";
            public const string LoginPath = "/Account/Login";
            public const string LoginLabel = "Login";
        }
        public struct Settings
        {
            public const string SiteConfiguration = "SiteConfiguration";
        }
    }
}
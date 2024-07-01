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
            public const string AboutPath = "/About";
            public const string AboutLabel = "About";
            public const string BrowsePath = "/Channels";
            public const string BrowseLabel = "Browse";
            public const string ProfilePath = "/Account/Profile";
            public const string ProfileLabel = "Profile";
            public const string PostsPath = "/Account/Posts";
            public const string PostsLabel = "Posts";
            public const string MediaPath = "/Account/Media";
            public const string MediaLabel = "Media";
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
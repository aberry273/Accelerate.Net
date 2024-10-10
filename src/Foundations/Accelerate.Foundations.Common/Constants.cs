using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common
{
    public struct Constants
    {
        public struct Global
        {
            public static Guid GlobalAdmin = Guid.Parse("00000000-0000-0000-0000-000000000000");
            public static Guid GlobalAdminContent = Guid.Parse("00000000-0000-0000-0000-000000000001");
            public static Guid GlobalAdminMedia = Guid.Parse("00000000-0000-0000-0000-000000000002");
            public static Guid GlobalAdminAccounts = Guid.Parse("00000000-0000-0000-0000-000000000003");
            public static Guid GlobalAdminOperations = Guid.Parse("00000000-0000-0000-0000-000000000004");
            public static Guid ChannelNewsGuid = Guid.Parse("d9dff599-0057-454c-bdb5-ebaad9855713");
        }
        public struct Domains
        {
            public const string Public = "Public";
            public const string Internal = "Internal";
        }
        public struct AdminPaths
        {
            public const string JobsPath = "/Admin/Jobs";
            public const string JobsLabel = "Jobs";
            public const string ActionsPath = "/Admin/Actions";
            public const string ActionsLabel = "Actions";
            public const string UsersPath = "/Admin/Users";
            public const string UsersLabel = "Users";
        }
        public struct Paths
        {
            public const string SearchPath = "/Search";
            public const string SearchLabel = "Search";
            public const string ChatLabel = "Chat";
            public const string AboutPath = "/About";
            public const string AboutLabel = "About";
            public const string FeedsPath = "/Feeds";
            public const string FeedsLabel = "Feeds";
            public const string ThreadsPath = "/Threads";
            public const string ThreadsLabel = "Threads";
            public const string ChatsPath = "/Chats";
            public const string ChatsLabel = "Chats";
            public const string ChannelsPath = "/Channels";
            public const string ChannelsLabel = "Channels";
            public const string ListsPath = "/Lists";
            public const string ListsLabel = "Lists";
            public const string ProfilePath = "/Account/Profile";
            public const string ProfileLabel = "Profile";
            public const string PostsPath = "/Account/Posts";
            public const string PostsLabel = "Posts";
            public const string NotificationsPath = "/Account/Notifications";
            public const string NotificationsLabel = "Notifications";
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
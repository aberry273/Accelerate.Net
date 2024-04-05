using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Websockets.Hubs
{
    public static class HubClientConnectionsSingleton
    {
        //Key=userId, value=list of connectionIds
        public static Dictionary<string, List<string>> UserConnections { get; set; } = new Dictionary<string, List<string>>();
        //Key=channelId, value=list of connectionIds
        public static Dictionary<string, List<string>> ChannelConnections { get; set; } = new Dictionary<string, List<string>>() { };

        //Key=threadId, value=list of connectionIds
        public static Dictionary<string, List<string>> ThreadConnections { get; set; } = new Dictionary<string, List<string>>();

        public static List<string> GetUserConnections(string userId)
        {
            return UserConnections.FirstOrDefault(x => x.Key == userId).Value;
        }
        public static List<string> GetChannelConnections(string channelId)
        {
            return ChannelConnections.FirstOrDefault(x => x.Key == channelId).Value;
        }
        public static List<string> GetThreadConnections(string channelId)
        {
            return ThreadConnections.FirstOrDefault(x => x.Key == channelId).Value;
        }
    }
}

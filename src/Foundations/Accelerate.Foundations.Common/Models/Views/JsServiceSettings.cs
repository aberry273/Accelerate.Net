using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Models.Views
{
    /*
     * wssContentChannels: {
                userId: '@Model.UserId',
                wssEvent: 'wss:contentChannels',
                url: "@Model.Url/ContentChannels",
            },
            wssContentPosts: {
                userId: '@Model.UserId',
                wssEvent: 'wss:contentPosts',
                url: "@Model.Url/ContentPosts",
                postbackUrl: '/api/contentpostactions',
                queryUrl: '/api/contentsearch/posts',
            },
            wssContentPostActions: {
                userId: '@Model.UserId',
                wssEvent: 'wss:contentPostActions',
                url: "@Model.Url/ContentPostActions",
                postbackUrl: '/api/contentpostactions',
                queryUrl: '/api/contentsearch/actions',
            },
            wssMediaBlobs: {
                userId: '@Model.UserId',
                wssEvent: 'wss:mediaBlobs',
                url: "@Model.Url/MediaBlobs",
            },
    */
    public class JsServiceSettings
    {
        public string ServiceName { get; set; }
        public string UserId { get; set; }
        public string WssEvent { get; set; }
        public string Url { get; set; }
        public string PostbackUrl { get; set; }
        public string QueryUrl { get; set; }
    }
}

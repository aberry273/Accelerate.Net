using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Content.Models.View;
using Accelerate.Foundations.Integrations.Elastic.Models;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Accelerate.Foundations.Content.Models.Data
{
    public class ContentPostQuoteSubdocument
    {
        public string ContentPostQuoteThreadId { get; set; }
        public string ContentPostQuoteId { get; set; }
        public string? Content { get; set; }
        public string? Response { get; set; }
    }
    public class ContentPostContentMediaSubdocument
    {
        public string Src { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
    public class ContentPostContentSubdocument
    {
        public string Date { get; set; }
        public string Text { get; set; }
        public List<ContentPostFormatItem> Formats { get; set; }
        public List<ContentPostContentMediaSubdocument> Media { get; set; }
    }
    public class ContentPostRelatedPostsSubdocument
    {
        public List<ContentPostQuoteSubdocument> Quotes { get; set; }
        public List<Guid> ParentIds { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? ChannelId { get; set; }
        public Guid? ChatId { get; set; }
        public Guid? ListId { get; set; }
    }
    public class ContentPostMetricsSubdocument
    {
        public int Replies { get; set; }
        public int Quotes { get; set; }
        public int Rating { get; set; }
    }
    public class ContentPostTaxonomySubdocument
    {
        public string Category { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Labels { get; set; }
    }
    public class ContentPostDocument : ContentEntityDocument
    {
        public string? ExternalId { get; set; }

       
        public string? Category
        {
            get
            {
                if(this.Taxonomy == null) return string.Empty;
                return this.Taxonomy.Category;
            }
        }
        public IEnumerable<string>? Tags
        {
            get
            {
                if (this.Taxonomy == null) return new List<string>();
                return this.Taxonomy.Tags;
            }
        }

        public ContentPostEntityStatus Status { get; set; }

        public ContentPostUserProfileSubdocument Profile { get; set; }
        public ContentPostContentSubdocument Content { get; set; }
        public virtual ContentPostRelatedPostsSubdocument Related { get; set; }
        public ContentPostQuoteSubdocument Quotes { get; set; }
        public ContentPostMetricsSubdocument Metrics { get; set; }
        public ContentPostTaxonomySubdocument Taxonomy { get; set; }
        public ContentPostLinkSubdocument Link { get; set; }
    }
    /*
     * id: 0,
            href: '#',
            profile: {
                icon: 'calendar',
                displayName: 'Deb peterson',
                username: '@johnwes',
                description: 'Blogger, thraser, skaterboi, etc',
                date: '10/07/2024',
                href: '#',
                img: 'https://flowbite.s3.amazonaws.com/blocks/marketing-ui/avatars/roberta-casas.png',
            },
            content: {
                date: '2024-07-20T00:38:52.0421225Z',
                text: 'Display users\' comments beautifully. The component also has a form for commenting.',
            },
            ui: {
                showReplies: true,
            },
            replies: {
                profiles: [
                    {
                        img: 'https://flowbite.s3.amazonaws.com/blocks/marketing-ui/avatars/roberta-casas.png',
                    },
                    {
                        img: 'https://flowbite.s3.amazonaws.com/blocks/marketing-ui/avatars/roberta-casas.png',
                    },
                    {
                        img: 'https://flowbite.s3.amazonaws.com/blocks/marketing-ui/avatars/roberta-casas.png',
                    },
                ],
                text: 'View replies (3)',
                date: '2024-07-20T00:38:52.0421225Z',
            },
            metrics: {
                replies: 4,
                rating: 17231
            },
            taxonomy: {
                category: 'Marketing',
            },
            menu: [ 'CopyLink', 'Edit', 'Delete' ],
            actions: [ 'reply', 'quote', 'upvote', 'downvote', 'tag' ],
    */
}

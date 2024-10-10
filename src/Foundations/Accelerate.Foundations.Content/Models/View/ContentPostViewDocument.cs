using Accelerate.Foundations.Common.Extensions;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Integrations.Elastic.Models;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Accelerate.Foundations.Content.Models.View
{
    public class ContentPostChannelViewSubdocument
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
    //View
    public class ContentPostUiSubdocument
    {
        public string Href { get; set; }
    }
    public class ContentPostUserProfileSubdocument
    {
        public Guid Id { get; set; }
        public string Icon { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string Img { get; set; }
        public string Href { get; set; }
    }
    //View
    public class ContentPostRepliesSubdocument
    {
        public List<ContentPostUserProfileSubdocument> Profiles { get; set; }
        public string Text { get; set; }
        public string Date { get; set; }
    }
    public class ContentPostViewDocument : ContentPostDocument
    {
        //View model only
        public ContentPostChannelViewSubdocument Channel { get; set; }
        public ContentPostUserProfileSubdocument Profile { get; set; }
        public ContentPostUiSubdocument Ui { get; set; } = new ContentPostUiSubdocument();
        public ContentPostRepliesSubdocument Replies { get; set; } = new ContentPostRepliesSubdocument();
        public List<string> Menu { get; set; }
        public List<string> Actions { get; set; }
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

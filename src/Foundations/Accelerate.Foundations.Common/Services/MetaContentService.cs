using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;
using Accelerate.Foundations.Common.Models.Views;

namespace Accelerate.Foundations.Common.Services
{
    public class MetaContentService : IMetaContentService
    {
        public BasePage CreatePageBaseContent(UserProfile? profile = null)
        {
            return (profile != null)
                ? this.CreateAuthenticatedContent()
                : this.CreateUnauthenticatedContent(); 
        }
        public BasePage CreateUnauthenticatedContent()
        {
            return new BasePage()
            {
                SideNavigation = new NavigationGroup(),
                Breadcrumbs = new List<NavigationItem>(),
                Footer = new Footer(),
                Metadata = new PageMetadata(),
                SocialMetadata = new SocialMetadata(),
                TopNavigation = CreateTopNavigation(),
                SEO = new SeoMetadata(),
            };
        }
        public BasePage CreateAuthenticatedContent()
        {
            return new BasePage()
            {
                SideNavigation = new NavigationGroup(),
                Breadcrumbs = new List<NavigationItem>(),
                Footer = new Footer(),
                Metadata = new PageMetadata(),
                SocialMetadata = new SocialMetadata(),
                TopNavigation = CreateAuthenticatedTopNavigation(),
                SEO = new SeoMetadata(),
            };
        }

        public NavigationGroup CreateTopNavigation()
        {
            return new NavigationGroup()
            {
                Title = "parrot",
                Subtitle = "The new bird in town",
                Items = new List<NavigationItem>()
                {
                    new NavigationItem()
                    {
                        Href = "/Account/Login",
                        Text = "Login",
                    },
                }
            };
        }

        public NavigationGroup CreateAuthenticatedTopNavigation()
        {
            return new NavigationGroup()
            {
                Title = "parrot",
                Subtitle = "The new bird in town",
                Items = new List<NavigationItem>()
                {
                    new NavigationItem()
                    {
                        Href = "/Content/Feed",
                        Text = "Feed",
                    },
                    new NavigationItem()
                    {
                        Href = "/Account/Manage",
                        Text = "Account",
                    },
                    new NavigationItem()
                    {
                        Href = "/Account/Logout",
                        Text = "Logout",
                    },
                }
            };
        }
    }
}

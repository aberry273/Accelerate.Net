using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Services
{
    public class SharedContentService : ISharedContentService
    {
        public BasePage CreatePageBaseContent()
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

        public NavigationGroup CreateTopNavigation()
        {
            return new NavigationGroup()
            {
                Title = "feed.at",
                Subtitle = "Opensource",
                Items = new List<NavigationItem>()
                {
                    new NavigationItem()
                    {
                        Href = "/Account/Login",
                        Text = "Login",
                    }
                }
            };
        }
    }
}

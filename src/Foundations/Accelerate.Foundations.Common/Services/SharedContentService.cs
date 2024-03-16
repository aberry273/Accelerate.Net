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
                TopNavigation = new NavigationGroup(),
                SEO = new SeoMetadata(),
            };
        }
    }
}

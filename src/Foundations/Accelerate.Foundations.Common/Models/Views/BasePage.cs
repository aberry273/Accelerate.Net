using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Models.Views
{
    public class BasePage
    {
        public Guid? UserId { get; set; }
        public bool IsAuthenticated { get; set; }
        public List<JsServiceSettings> ServiceSettings { get; set; } = new List<JsServiceSettings>();
        public NavigationBar TopNavigation { get; set; } = new NavigationBar();

        public Footer Footer { get; set; } = new Footer();

        public NavigationGroup SideNavigation { get; set; } = new NavigationGroup();

        public List<NavigationItem> Breadcrumbs { get; set; } = new List<NavigationItem>();

        public PageMetadata Metadata { get; set; } = new PageMetadata();

        public SocialMetadata SocialMetadata { get; set; } = new SocialMetadata();

        public SeoMetadata SEO { get; set; } = new SeoMetadata();

        public string? Url { get; set; }

        public string? UrlAbsolute { get; set; }
        public BasePage() { }
        public BasePage(BasePage model)
        {
            UserId = model.UserId;
            IsAuthenticated = model.IsAuthenticated;
            Breadcrumbs = model.Breadcrumbs;
            Footer = model.Footer;
            TopNavigation = model.TopNavigation;
            Metadata = model.Metadata;
            SEO = model.SEO;
            SideNavigation = model.SideNavigation;
            SocialMetadata = model.SocialMetadata;
            ServiceSettings = model.ServiceSettings;
        }
    }
}

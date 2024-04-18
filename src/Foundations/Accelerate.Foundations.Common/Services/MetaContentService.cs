using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;
using Accelerate.Foundations.Common.Models.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Azure.Core;

namespace Accelerate.Foundations.Common.Services
{
    public class MetaContentService : IMetaContentService
    {
        protected IUrlHelper _urlHelper;
        SiteConfiguration _siteConfig;
        public MetaContentService(
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccessor,
            IOptions<SiteConfiguration> siteConfig)
        {
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
            _siteConfig = siteConfig.Value;
        }

        public string GetCurrentUrl()
        {
            var Request = _urlHelper.ActionContext.HttpContext.Request;
            return $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";

        }
        public string GetActionUrl(string action, string controller, object values = null, string protocol = null)
        {
            return _urlHelper.Action(
                action: action,
                controller: controller,
                values: values,
                protocol: (protocol ?? this._urlHelper.ActionContext.HttpContext.Request.Scheme));
        }
        public BasePage CreatePageBaseContent(UserProfile? profile = null)
        {
            return (profile != null)
                ? this.CreateAuthenticatedContent(profile)
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
        public BasePage CreateAuthenticatedContent(UserProfile? profile)
        {
            return new BasePage()
            {
                SideNavigation = new NavigationGroup(),
                Breadcrumbs = new List<NavigationItem>(),
                Footer = new Footer(),
                Metadata = new PageMetadata(),
                SocialMetadata = new SocialMetadata(),
                TopNavigation = CreateAuthenticatedTopNavigation(profile),
                SEO = new SeoMetadata(),
            };
        }

        public NavigationBar CreateTopNavigation()
        {
            return new NavigationBar()
            {
                Title = _siteConfig.Name,
                Href = _siteConfig.Domain,
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

        public NavigationBar CreateAuthenticatedTopNavigation(UserProfile? profile)
        {
            return new NavigationBar()
            {
                Authenticated = true,
                Title = _siteConfig.Name,
                Href = _siteConfig.Domain,
                Subtitle = "The new bird in town",
                Dropdown = profile == null ? null : new NavigationAvatarDropdown()
                {
                    Image = profile?.Image,
                    Items = new List<NavigationItem>()
                    {
                        new NavigationItem()
                        {
                            Disabled = true,
                            Text = profile?.Username,
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
                },
                Items = new List<NavigationItem>()
                {
                    new NavigationItem()
                    {
                        Href = "/Content/Browse",
                        Text = "Browse",
                    },
                }
            };
        }
    }
}

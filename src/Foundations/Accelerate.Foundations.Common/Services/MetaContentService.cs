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
using System.Security.Principal;
using System.Security.Claims;

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
            return (profile != null && profile.IsAuthenticated)
                ? this.CreateAuthenticatedContent(profile)
                : this.CreateUnauthenticatedContent();
        }
        private List<JsServiceSettings> CreateContentServiceSettings(Guid? userId, string domainUrl)
        {
            return new List<JsServiceSettings>()
            {
                new JsServiceSettings()
                {
                    ServiceName = "wssContentChannels",
                    UserId = userId.ToString(),
                    WssEvent = "wss:contentChannels",
                    Url = $"{domainUrl}/ContentChannels",
                },
                new JsServiceSettings()
                {
                    ServiceName = "wssContentPosts",
                    UserId = userId.ToString(),
                    WssEvent = "wss:contentPosts",
                    Url = $"{domainUrl}/ContentPosts",
                    PostbackUrl = "/api/contentposts",
                    QueryUrl = "/api/contentsearch/posts"
                },
                new JsServiceSettings()
                {
                    ServiceName = "wssContentPostActions",
                    UserId = userId.ToString(),
                    WssEvent = "wss:contentPostActions",
                    Url = $"{domainUrl}/ContentPostActions",
                    PostbackUrl = "/api/contentpostactions",
                    QueryUrl = "/api/contentsearch/actions"
                },
                new JsServiceSettings()
                {
                    ServiceName = "wssMediaBlobs",
                    UserId = userId.ToString(),
                    WssEvent = "wss:mediaBlobs",
                    Url = $"{domainUrl}/MediaBlobs",
                },
            };
        }
        public BasePage CreateUnauthenticatedContent()
        {
            return new BasePage()
            {
                Url = _siteConfig.Domain,
                ServiceSettings = this.CreateContentServiceSettings(null, _siteConfig.Domain),
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
                UserId = profile.UserId,
                IsAuthenticated = true,
                ServiceSettings = this.CreateContentServiceSettings(profile.UserId, _siteConfig.Domain),
                Url = _siteConfig.Domain,
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
                        Href = Foundations.Common.Constants.Paths.BrowsePath,
                        Text = Foundations.Common.Constants.Paths.BrowseLabel,
                    },
                    new NavigationItem()
                    {
                        Href = Foundations.Common.Constants.Paths.LoginPath,
                        Text = Foundations.Common.Constants.Paths.LoginLabel,
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
                            Href = Foundations.Common.Constants.Paths.ProfilePath,
                            Text = Foundations.Common.Constants.Paths.ProfileLabel,
                        },
                        new NavigationItem()
                        {
                            Href = Foundations.Common.Constants.Paths.LogoutPath,
                            Text = Foundations.Common.Constants.Paths.LogoutLabel,
                        },
                    }
                },
                Items = new List<NavigationItem>()
                {
                    new NavigationItem()
                    {
                        Href = Foundations.Common.Constants.Paths.BrowsePath,
                        Text = Foundations.Common.Constants.Paths.BrowseLabel,
                    },
                }
            };
        }
    }
}

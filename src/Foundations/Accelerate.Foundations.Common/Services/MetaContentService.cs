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
            _urlHelper = actionContextAccessor.ActionContext != null
                ? urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext)
                : null;
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
                    ServiceName = "wssSvcPosts",
                    UserId = userId.ToString(),
                    WssEvent = "wss:contentPosts",
                    Url = $"{domainUrl}/ContentPosts",
                    PostbackUrl = "/api/contentposts",
                    QueryUrl = "/api/contentsearch/posts"
                },
                /*
                new JsServiceSettings()
                {
                    ServiceName = "wssContentChannels",
                    UserId = userId.ToString(),
                    WssEvent = "wss:contentChannels",
                    Url = $"{domainUrl}/ContentChannels",
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
                new JsServiceSettings()
                {
                    ServiceName = "wssContentPostActionsSummary",
                    UserId = userId.ToString(),
                    WssEvent = "wss:contentPostActionsSummary",
                    Url = $"{domainUrl}/ContentPostActionsSummary",
                },
                new JsServiceSettings()
                {
                    ServiceName = "wssContentPostActivities",
                    UserId = userId.ToString(),
                    WssEvent = "wss:contentPostActivities",
                    Url = $"{domainUrl}/ContentPostActivities",
                },*/
            };
        }
        public BasePage CreateUnauthenticatedContent()
        {
            return new BasePage()
            {
                Url = _siteConfig.Domain,
                ServiceSettings = this.CreateContentServiceSettings(null, _siteConfig.Domain),
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
                IsAuthenticated = profile.IsAuthenticated,
                IsDeactivated = profile.IsDeactivated,
                ServiceSettings = this.CreateContentServiceSettings(profile.UserId, _siteConfig.Domain),
                Url = _siteConfig.Domain,
                SideNavigation = profile.Domain == Constants.Domains.Internal 
                    ? CreateInternalSideNavigation()
                    : CreatePublicSideNavigation(),
                Breadcrumbs = new List<NavigationItem>(),
                Footer = new Footer(),
                Metadata = new PageMetadata(),
                SocialMetadata = new SocialMetadata(),
                TopNavigation = CreateAuthenticatedTopNavigation(profile),
                SEO = new SeoMetadata(),
            };
        }
        public NavigationGroup CreateInternalSideNavigation()
        {
            return new NavigationGroup
            {
                Items = new List<NavigationItem>()
                {
                    new NavigationItem()
                    {
                        Icon = "queueList",
                        Text = Foundations.Common.Constants.AdminPaths.JobsLabel,
                        Href = Foundations.Common.Constants.AdminPaths.JobsPath
                    },
                    new NavigationItem()
                    {
                        Icon = "cog",
                        Text = Foundations.Common.Constants.AdminPaths.ActionsLabel,
                        Href = Foundations.Common.Constants.AdminPaths.ActionsPath
                    },
                    new NavigationItem()
                    {
                        Icon = "userGroup",
                        Text = Foundations.Common.Constants.AdminPaths.UsersLabel,
                        Href = Foundations.Common.Constants.AdminPaths.UsersPath
                    },
                }
            };
        }

        public NavigationGroup CreatePublicSideNavigation()
        {
            return new NavigationGroup
            {
                Items = new List<NavigationItem>()
                {
                    new NavigationItem()
                    {
                        Icon = "funnel",
                        Text = Foundations.Common.Constants.Paths.FeedsLabel,
                        Href = Foundations.Common.Constants.Paths.FeedsPath,
                    },
                    new NavigationItem()
                    {
                        Icon = "chatBubbles",
                        Text = Foundations.Common.Constants.Paths.ChatsLabel,
                        Href = Foundations.Common.Constants.Paths.ChatsPath,
                    },
                    new NavigationItem()
                    {
                        Icon = "queueList",
                        Text = Foundations.Common.Constants.Paths.ListsLabel,
                        Href = Foundations.Common.Constants.Paths.ListsPath,
                    },
                    new NavigationItem()
                    {
                        Icon = "chatBubble",
                        Text = Foundations.Common.Constants.Paths.ThreadsLabel,
                        Href = Foundations.Common.Constants.Paths.ThreadsPath,
                    },
                    new NavigationItem()
                    {
                        Icon = "rectangleStack",
                        Text = Foundations.Common.Constants.Paths.ChannelsLabel,
                        Href = Foundations.Common.Constants.Paths.ChannelsPath,
                    },
                }
            };
        }

        public NavigationBar CreateTopNavigation()
        {
            return new NavigationBar()
            {
                Logo = "/src/images/logo.png",
                Title = _siteConfig.Name,
                Href = _siteConfig.Domain,
                Subtitle = "Chat for familes and friends",
                PrimaryItems = new List<NavigationItem>()
                {
                    new NavigationItem()
                    {
                        Href = Foundations.Common.Constants.Paths.AboutPath,
                        Text = Foundations.Common.Constants.Paths.AboutLabel,
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
                Logo = "/src/images/logo.png",
                Title = _siteConfig.Name,
                Href = _siteConfig.Domain,
                Subtitle = "Chat for familes and friends",
                PrimaryItems = new List<NavigationItem>()
                {
                    new NavigationItem()
                    {
                        Href = Foundations.Common.Constants.Paths.FeedsPath,
                        Text = Foundations.Common.Constants.Paths.ChatLabel,
                    },
                    new NavigationItem()
                    {
                        Href = Foundations.Common.Constants.Paths.AboutPath,
                        Text = Foundations.Common.Constants.Paths.AboutLabel,
                    },
                    new NavigationItem()
                    {
                        Href = Foundations.Common.Constants.Paths.SearchPath,
                        Text = Foundations.Common.Constants.Paths.SearchLabel,
                    },
                },
                Dropdown = profile == null ? null : new NavigationAvatarDropdown()
                {
                    Title = profile?.Name,
                    Subtitle = profile?.Username,
                    Image = $"{profile?.Image}?w=50",
                    Groups = new List<NavigationGroup>()
                    {
                        new NavigationGroup()
                        {
                            Title = "Account",
                            Items = new List<NavigationItem>() {
                                new NavigationItem()
                                {
                                    Href = Foundations.Common.Constants.Paths.ProfilePath,
                                    Text = Foundations.Common.Constants.Paths.ProfileLabel,
                                },
                                new NavigationItem()
                                {
                                    Href = Foundations.Common.Constants.Paths.PostsPath,
                                    Text = Foundations.Common.Constants.Paths.PostsLabel,
                                },
                                new NavigationItem()
                                {
                                    Href = Foundations.Common.Constants.Paths.MediaPath,
                                    Text = Foundations.Common.Constants.Paths.MediaLabel,
                                },
                                new NavigationItem()
                                {
                                    Href = Foundations.Common.Constants.Paths.NotificationsPath,
                                    Text = Foundations.Common.Constants.Paths.NotificationsLabel,
                                },
                                new NavigationItem()
                                {
                                    Href = Foundations.Common.Constants.Paths.LogoutPath,
                                    Text = Foundations.Common.Constants.Paths.LogoutLabel,
                                },
                            }
                        }
                    }
                }
            };
        }
    }
}

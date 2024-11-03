using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Users.Services;

namespace Accelerate.Foundations.Portal.Services
{
    public class PortalContentService : IPortalContentService
    {
        public IPortalSessionService _portalSessionService { get; set; }
        public IMetaContentService _metaContentService { get; set; }
        public IUsersUserService _userService { get; set; }
        public PortalContentService(
            IPortalSessionService portalSessionService,
            IMetaContentService metaContentService,
            IUsersUserService userService)
        {
            _portalSessionService = portalSessionService;
            _metaContentService = metaContentService;
            _userService= userService;
        }
         
        public async Task<BasePage> CreateAuthenticatedContent(UsersUser user)
        {
            var profile = Foundations.Users.Helpers.UsersHelpers.CreateUserProfile(user);
            var basePage = _metaContentService.CreatePageBaseContent(profile);

            if((profile != null && profile.IsAuthenticated))
            {
                basePage.SideNavigation = profile.Domain == Foundations.Common.Constants.Domains.Internal
                   ? CreateAdminSideNavigation(user)
                   : await CreateCustomerSideNavigation(user);
            }

            return basePage;
        }
        public NavigationGroup CreateAdminSideNavigation(UsersUser user)
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
                    new NavigationItem()
                    {
                        Icon = "userGroup",
                        Text = Foundations.Common.Constants.AdminPaths.UsersLabel,
                        Href = Foundations.Common.Constants.AdminPaths.UsersPath
                    },
                }
            };
        }

        public async Task<NavigationGroup> CreateCustomerSideNavigation(UsersUser user)
        {
            try
            {
                if (await _userService.UserInRole(user, Constants.Roles.AccountIndividual))
                {
                    return await CreateCustomerBusinessSideNavigation(user);
                };
                if (await _userService.UserInRole(user, Constants.Roles.AccountBusiness))
                {
                    return await CreateCustomerBusinessSideNavigation(user);
                };
            }
            catch(Exception ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
            }
            return null; 
        }

        public async Task<NavigationGroup> CreateCustomerBusinessSideNavigation(UsersUser user)
        {
            var model = new NavigationGroup
            {
                Items = new List<NavigationItem>()
                {

                }
            };
            if (string.IsNullOrEmpty(_portalSessionService.TryGetSelectedAccountId()) 
                || !await _userService.UserInRole(user, Constants.Roles.AccountCreated))
            {
                model.Items.Add(
                    new NavigationItem()
                    {
                        Icon = "identification",
                        Text = Foundations.Common.Constants.Paths.OnboardingLabel,
                        Href = Foundations.Common.Constants.Paths.OnboardingPath,
                    }
                );
            }
            else
            {
                var nav = new List<NavigationItem>()
                {
                    new NavigationItem()
                    {
                        Icon = "buildingLibrary",
                        Text = Foundations.Common.Constants.Paths.AccountsLabel,
                        Href = Foundations.Common.Constants.Paths.AccountsPath,
                    },
                    new NavigationItem()
                    {
                        Icon = "bankNotes",
                        Text = Foundations.Common.Constants.Paths.TransactionsLabel,
                        Href = Foundations.Common.Constants.Paths.TransactionsPath,
                    },
                    new NavigationItem()
                    {
                        Icon = "currencyDollar",
                        Text = Foundations.Common.Constants.Paths.FundingLabel,
                        Href = Foundations.Common.Constants.Paths.FundingPath,
                    },
                    new NavigationItem()
                    {
                        Icon = "documentCurrency",
                        Text = Foundations.Common.Constants.Paths.SettlementsLabel,
                        Href = Foundations.Common.Constants.Paths.SettlementsPath,
                    },

                };
                model.Items.AddRange(nav);
            }

            return model;
        }
        public NavigationGroup CreateCustomerIndividualSideNavigation(UsersUser user)
        {
            var model = new NavigationGroup
            {
                Items = new List<NavigationItem>()
                {

                }
            };

            return model;
        }
    }
}

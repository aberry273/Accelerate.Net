using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Common.Helpers;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.Views;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Database.Models;
using Elastic.Clients.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Threading.Channels;
using Accelerate.Foundations.Portal.Services;
using Accelerate.Features.Accounts.Services;
using Accelerate.Features.Accounts.Models.Views;

namespace Accelerate.Features.Admin.Services
{
    public class AccountsBaseEntityViewService<T> : IAccountsBaseEntityViewService<T> where T : IBaseEntity
    {
        IMetaContentService _metaContentService;
        IPortalContentService _portalContentService;
        protected string ItemUrl { get; set; } 
        protected string EntityName { get; set; }
        protected string ApiUrl { get; set; }
        protected string EventDelete { get { return $"on:{this.EntityName.ToLower()}:delete"; } }
        protected string EventEdit { get { return $"on:{this.EntityName.ToLower()}:edit"; } }
        public AccountsBaseEntityViewService(
            IMetaContentService metaContentService,
            IPortalContentService portalContentService
            )
        {
            _metaContentService = metaContentService;
            _portalContentService = portalContentService;
            this.ApiUrl = "/api/contentsearch/posts";
            //ItemUrl = this._metaContentService.GetActionUrl(nameof(FeedsController.Feed), ControllerHelper.NameOf<FeedsController>(), new { id = x.Id });
        }

        private async Task<AccountsBasePage<T>> CreateBaseAccountsPage(UsersUser user)
        {
            var baseModel = await _portalContentService.CreateAuthenticatedContent(user);
            var viewModel = new AccountsBasePage<T>(baseModel);
            return viewModel;
        }
        public async Task<AccountsNotFoundPage> CreateNotFoundPage(UsersUser user, string title = null, string description = null)
        {
            var model = await CreateBaseAccountsPage(user);
            var viewModel = new AccountsNotFoundPage(model);
            viewModel.ReturnLink = GetReturnLink();
            viewModel.Title = title ?? "Page not found";
            viewModel.Description = description ?? "We are unable to retrieve this page, it may have been deleted or made private.";
            return viewModel;
        }

        public virtual async Task<AccountsBasePage<T>> CreateListPage(UsersUser user, IEnumerable<T> items)
        {
            var viewModel = await CreateBaseAccountsPage(user);
            //var viewModel = new AdminCreatePage(model);
            var pageName = "All";
            viewModel.SideNavigation.Selected = $"{this.EntityName}s";


            var links = CreatePageNavigationGroup(this.EntityName, pageName);
            links.Items.AddRange(GetLinks(items));
            viewModel.PageLinks = new List<NavigationGroup>()
            {
                links
            };

            viewModel.PageActions = CreatePageActionsGroup(this.EntityName, pageName);
             
            viewModel.UserId = user != null ? user.Id : null;
            //viewModel.FormCreatePost = user != null ? CreateForm(user) : null;
            return viewModel;
        }
        public virtual async Task<AccountsBasePage<T>> CreateIndexPage(UsersUser user)
        {
            var viewModel = await CreateBaseAccountsPage(user);
            //var viewModel = new AdminCreatePage(model); 
            var pageName = "All";
            viewModel.SideNavigation.Selected = $"{this.EntityName}s";

            var links = CreatePageNavigationGroup(this.EntityName, pageName);
           
            viewModel.PageLinks = new List<NavigationGroup>()
            {
                links
            };

            viewModel.PageActions = CreatePageActionsGroup(this.EntityName, pageName);
             
            viewModel.UserId = user != null ? user.Id : null;
            //viewModel.FormCreatePost = user != null ? CreateForm(user) : null;
            return viewModel;
        }
        public virtual string GetEntityName(T item)
        {
            return item.Id.ToString();
        }

        public virtual async Task<AccountsBasePage<T>> CreateEntityPage(UsersUser user, T item)
        {
            var viewModel = await CreateBaseAccountsPage(user);
            //var viewModel = new AdminIndexPage<T>(model);
            viewModel.Id = item.Id;
            var pageName = GetEntityName(item);
            viewModel.SideNavigation.Selected = $"{this.EntityName}s";

            var links = CreatePageNavigationGroup(this.EntityName, pageName);
       
            viewModel.PageLinks = new List<NavigationGroup>()
            {
                links
            };

            viewModel.PageActions = CreatePageActionsGroup(this.EntityName, pageName, item); 
           
            viewModel.ModalDelete = CreateModalDeleteForm(user, item);
            
            viewModel.UserId = user.Id;
            //viewModel.Form = CreateEntityForm(user, item, PostbackType.PUT);
            return viewModel;
        }
        public virtual async Task<AccountsCreatePage> CreateNewPage(UsersUser user)
        {
            var model = await CreateBaseAccountsPage(user);
            var viewModel = new AccountsCreatePage(model);
            var pageName = $"Create {this.EntityName}";
            
            viewModel.SideNavigation.Selected = $"{this.EntityName}s";


            var links = CreatePageNavigationGroup(this.EntityName, pageName);
           
            viewModel.PageLinks = new List<NavigationGroup>()
            {
                links
            };

            viewModel.PageActions = CreatePageActionsGroup(this.EntityName, pageName);

            viewModel.UserId = user != null ? user.Id : null;
            //viewModel.Form = CreateForm(user);
            return viewModel;
        } 

        public virtual async Task<AccountsBasePage<T>> CreateUpdatePage(UsersUser user, T item)
        {
            var viewModel = await CreateBaseAccountsPage(user);
            //var viewModel = new AdminIndexPage<T>(model);
            var pageName = $"Edit {this.GetEntityName(item)}";
              viewModel.SideNavigation.Selected = $"{this.EntityName}s";

            var links = CreatePageNavigationGroup(this.EntityName, pageName);
    
            viewModel.PageLinks = new List<NavigationGroup>()
            {
                links
            };

            viewModel.PageActions = CreatePageActionsGroup(this.EntityName, pageName);
            viewModel.ModalDelete = CreateModalDeleteForm(user, item);

            viewModel.UserId = user?.Id;
            //viewModel.Form = CreateEntityForm(user, item, PostbackType.PUT);
            return viewModel;
        }

        private NavigationGroup CreatePageNavigationGroup(string entity, string selected)
        {
            var plural = $"{entity}s";
            return new NavigationGroup()
            {
                Title = "All",
                Selected = selected,
                Items = new List<NavigationItem>()
                {
                    new NavigationItem()
                    {
                        Text = $"Create {entity}",
                        Href = $"/Admin/{plural}/Create",
                        Class = "relative flex cursor-pointer select-none items-center rounded px-2 py-1.5 text-sm text-gray-500 hover:text-gray-900 hover:bg-gray-200 dark:text-gray-400 dark:hover:text-white dark:hover:bg-gray-600 data-[disabled]:pointer-events-none data-[disabled]:opacity-50"
                    },
                    new NavigationItem()
                    {
                        Text = "All",
                        Href = $"/Admin/{plural}",
                    }
                }
            };
        }
        private ButtonGroup CreatePageActionsGroup(string entity, string name)
        {
            var plural = $"{entity}s";
            return new ButtonGroup()
            {
                Title = name,
                Items = new List<ButtonItem>()
                {
                }
            };
        }
        private ButtonGroup CreatePageActionsGroup(string entity, string name, IBaseEntity item)
        {
            var plural = $"{entity}s";
            return new ButtonGroup()
            {
                Title = name,
                Items = new List<ButtonItem>()
                {
                    new ButtonItem()
                    {
                        Text = $"Edit",
                        Href = $"/Admin/{plural}/{item.Id}/Edit",
                        Class = "relative flex cursor-pointer select-none items-center rounded px-2 py-1.5 text-sm text-gray-500 hover:text-gray-900 hover:bg-gray-200 dark:text-gray-400 dark:hover:text-white dark:hover:bg-gray-600 data-[disabled]:pointer-events-none data-[disabled]:opacity-50",
                        Icon = "edit"
                    },
                    new ButtonItem()
                    {
                        Text = $"Delete",
                        Event = EventDelete,
                        Class = "relative flex cursor-pointer select-none items-center rounded px-2 py-1.5 text-sm text-gray-500 hover:text-gray-900 hover:bg-gray-200 dark:text-gray-400 dark:hover:text-white dark:hover:bg-gray-600 data-[disabled]:pointer-events-none data-[disabled]:opacity-50",
                        Icon = "trash"
                    },
                    new ButtonItem()
                    {
                        Text = $"Search",
                        Href = $"/Admin/{plural}/{item.Id}/Search",
                        Class = "relative flex cursor-pointer select-none items-center rounded px-2 py-1.5 text-sm text-gray-500 hover:text-gray-900 hover:bg-gray-200 dark:text-gray-400 dark:hover:text-white dark:hover:bg-gray-600 data-[disabled]:pointer-events-none data-[disabled]:opacity-50",
                        Icon = "magnifyingGlass"
                    },
                }
            };
        }
        public NavigationItem? GetReturnLink()
        {
            return new NavigationItem()
            {
                Text = "Return",
                Href = "#"//this._metaContentService.GetActionUrl(nameof(ChannelsController.Index), ControllerHelper.NameOf<ChannelsController>(), new { })
            };
        }   

        public ModalForm CreateModalDeleteForm(UsersUser user, T item)
        {
            var model = new ModalForm();
            model.Title = $"Delete {this.EntityName}";
            model.Event = EventDelete;
            model.Form = CreateFormDelete(user, item);
            return model;
        }
        public AjaxForm CreateFormDelete(UsersUser user, T item)
        {
            var model = new AjaxForm()
            {
                Action = $"/api/admin{this.EntityName.ToLower()}/{item.Id}",
                Type = PostbackType.DELETE,
                Event = $"{this.EntityName.ToLower()}:deleted:modal",
                Label = "Delete",
                Fields = new List<FormField>()
                {
                    new FormField()
                    {
                        Name = "Id",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        AriaInvalid = false,
                        Value = item.Id,
                    },
                    new FormField()
                    {
                        Name = "UserId",
                        FieldType = FormFieldTypes.input,
                        Hidden = true,
                        Disabled = true,
                        AriaInvalid = false,
                        Value = user.Id,
                    }
                }
            };
            return model;
        }

        public List<NavigationItem> GetLinks(IEnumerable<T> searchResponse = null, string selectedName = null)
        {
            var model = new List<NavigationItem>();
            if (searchResponse != null && searchResponse.Any())
            {
                var channelItems = searchResponse.Select(GetLink);
                model.AddRange(channelItems);
            }
            return model;
        }
        public NavigationItem? GetLink(T x)
        {
            if (x == null) { return null; }
            return new NavigationItem()
            {
                Text = $"{this.GetEntityName(x)}",
                Href = $"/Accounts/{this.EntityName}s/{x.Id}",
            };
        }
    }
}

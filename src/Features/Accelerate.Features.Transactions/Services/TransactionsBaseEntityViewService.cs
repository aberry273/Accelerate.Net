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
using Accelerate.Features.Transactions.Services;
using Accelerate.Features.Transactions.Models.Views;
using Accelerate.Foundations.Transactions.Models.Entities;

namespace Accelerate.Features.Admin.Services
{
    public class TransactionsBaseEntityViewService<T> : ITransactionsBaseEntityViewService<T> where T : IBaseEntity
    {
        IMetaContentService _metaContentService;
        IPortalContentService _portalContentService;
        protected string ItemUrl { get; set; } 
        protected string EntityName { get; set; }
        protected string ApiUrl { get; set; }
        protected string EventDelete { get { return $"on:{this.EntityName.ToLower()}:delete"; } }
        protected string EventEdit { get { return $"on:{this.EntityName.ToLower()}:edit"; } }
        public TransactionsBaseEntityViewService(
            IMetaContentService metaContentService,
            IPortalContentService portalContentService
            )
        {
            _metaContentService = metaContentService;
            _portalContentService = portalContentService;
            this.ApiUrl = "/api/contentsearch/posts";
            //ItemUrl = this._metaContentService.GetActionUrl(nameof(FeedsController.Feed), ControllerHelper.NameOf<FeedsController>(), new { id = x.Id });
        }

        private async Task<TransactionsBasePage<T>> CreateBaseTransactionsPage(UsersUser user)
        {
            var baseModel = await _portalContentService.CreateAuthenticatedContent(user);
            var viewModel = new TransactionsBasePage<T>(baseModel);
            return viewModel;
        }
        public async Task<TransactionsNotFoundPage> CreateNotFoundPage(UsersUser user, string title = null, string description = null)
        {
            var model = await CreateBaseTransactionsPage(user);
            var viewModel = new TransactionsNotFoundPage(model);
            viewModel.ReturnLink = GetReturnLink();
            viewModel.Title = title ?? "Page not found";
            viewModel.Description = description ?? "We are unable to retrieve this page, it may have been deleted or made private.";
            return viewModel;
        }

        public virtual async Task<TransactionsBasePage<T>> CreateListPage(UsersUser user, IEnumerable<T> items)
        {
            var viewModel = await CreateBaseTransactionsPage(user);
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
        public virtual async Task<TransactionsBasePage<T>> CreateIndexPage(UsersUser user)
        {
            var viewModel = await CreateBaseTransactionsPage(user);
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

        public virtual async Task<TransactionsBasePage<T>> CreateEntityPage(UsersUser user, T item)
        {
            var viewModel = await CreateBaseTransactionsPage(user);
            //var viewModel = new AdminIndexPage<T>(model);
            viewModel.Id = item.Id;
            viewModel.Entity = item;
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
        public virtual async Task<TransactionsFormPage> CreateNewPage(UsersUser user)
        {
            var model = await CreateBaseTransactionsPage(user);
            var viewModel = new TransactionsFormPage(model);
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

        public virtual async Task<TransactionsFormPage> CreateUpdatePage(UsersUser user, T item)
        {
            var model = await CreateBaseTransactionsPage(user);
            var viewModel = new TransactionsFormPage(model);
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
            viewModel.Form = CreateEntityForm(user, item);
            return viewModel;
        }

        public AjaxForm CreateAddressForm(UsersUser user, T item)
        {
            var model = this.CreateEntityForm(user, item, PostbackType.POST);
            model.Label = $"Create Address";
            model.Fields = CreateAddressFormFields(user);
            return model;
        }

        public virtual AjaxForm CreateEntityForm(UsersUser user, T? item, PostbackType type = PostbackType.POST)
        {
            var model = new AjaxForm()
            {
                Action = item == null
                    ? $"{Foundations.Common.Constants.ApiPaths.VersionPath}/Transactions/{this.EntityName.ToLower()}"
                    : $"{Foundations.Common.Constants.ApiPaths.VersionPath}/Transactions/{this.EntityName.ToLower()}/{item.Id}",
                Type = type,
                Event = $"{this.EntityName.ToLower()}:create:modal",
                Label = "Create",
                Fields = new List<FormField>()
            };
            if (item != null)
            {
                model.Fields.Add(
                    new FormField()
                    {
                        Name = "Id",
                        FieldType = FormFieldTypes.input,
                        AriaInvalid = false,
                        Value = item.Id,
                        Hidden = true,
                        Disabled = true
                    }
                );
            }
            return model;
        }
        public List<FormField> CreateAddressFormFields(UsersUser user)
        {
            return new List<FormField>()
            {
                _metaContentService.FormField("StreetAddress1", FormFieldComponents.aclFieldInput, null, null, null, false, false, null, null, null, "Firstname"),
                _metaContentService.FormField("StreetAddress2", FormFieldComponents.aclFieldInput, null, null, null, false, false, null, null, null, "Lastname"),
                _metaContentService.FormField("Postcode", FormFieldComponents.aclFieldInput, null, null, null, false, false, null, null, null, "Email"),
                _metaContentService.FormField("Suburb", FormFieldComponents.aclFieldInput, null, null, null, false, false, null, null, null, "Email"),
                _metaContentService.FormField("City", FormFieldComponents.aclFieldInput, null, null, null, false, false, null, null, null, "Email"),
                _metaContentService.FormField("Region", FormFieldComponents.aclFieldInput, null, null, null, false, false, null, null, null, "Email"),
                _metaContentService.FormField("Country", FormFieldComponents.aclFieldInput, null, null, null, false, false, null, null, null, "Email"),
            };
        }

        private NavigationGroup CreatePageNavigationGroup(string entity, string selected)
        {
            return new NavigationGroup()
            {
                Title = "All",
                Selected = selected,
                Items = new List<NavigationItem>()
                {
                    new NavigationItem()
                    {
                        Text = $"Create {entity}",
                        Href = $"/Transactions/{entity}/Create",
                        Class = "relative flex cursor-pointer select-none items-center rounded px-2 py-1.5 text-sm text-gray-500 hover:text-gray-900 hover:bg-gray-200 dark:text-gray-400 dark:hover:text-white dark:hover:bg-gray-600 data-[disabled]:pointer-events-none data-[disabled]:opacity-50"
                    },
                    new NavigationItem()
                    {
                        Text = "All",
                        Href = $"/Transactions/{entity}",
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
            return new ButtonGroup()
            {
                Title = name,
                Items = new List<ButtonItem>()
                {
                    new ButtonItem()
                    {
                        Text = $"Edit",
                        Href = $"/Transactions/{entity}/{item.Id}/Edit",
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
                        Href = $"/Transactions/{entity}/{item.Id}/Search",
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
                Action = $"/api/Transactions{this.EntityName.ToLower()}/{item.Id}",
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
                Href = $"/Transactions/{this.EntityName}s/{x.Id}",
            };
        }
    }
}

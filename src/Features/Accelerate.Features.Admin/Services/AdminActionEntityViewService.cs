using Accelerate.Features.Admin.Models.Views; 
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.View;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Operations.Models.Entities;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using System;
using System.Linq;
using static MassTransit.Transports.ReceiveEndpoint;

namespace Accelerate.Features.Admin.Services
{
    public class AdminActionEntityViewService : AdminBaseEntityViewService<OperationsActionEntity>
    {
        public AdminActionEntityViewService(
            IMetaContentService metaContent)
            : base(metaContent)
        {
            EntityName = "Action";
        }

        public override string GetEntityName(OperationsActionEntity item)
        {
            return item.Name;
        }

        public AjaxForm CreateActionForm(AccountUser user)
        {
            var model = base.CreateForm(user);

            model.Fields = CreateFormFields(user, null);

            return model;
        }
        public override AdminCreatePage CreateAddPage(AccountUser user, IEnumerable<OperationsActionEntity> items)
        {
            var viewModel = base.CreateAddPage(user, items);
            viewModel.Form = CreateActionForm(user);
            return viewModel;
        }

        private List<KeyValuePair<string, string>> GetActionStateItems()
        {
            var values = Enum.GetNames<OperationsJobState>().ToList();
            var kv = values.Select(x => new KeyValuePair<string, string>(
                x,
                ((int)Enum.Parse<OperationsJobState>(x, true)).ToString()
            )).ToList();
            return kv;
        }
        private string GetStateItem(OperationsActionEntity item)
        {
            return Enum.GetName<OperationsActionState>(item?.State ?? OperationsActionState.Public);
        }
        public List<FormField> CreateFormFields(AccountUser user, OperationsActionEntity? item)
        {
            var actionValue = item != null
                ? item?.Action
                : Foundations.Operations.Constants.Settings.Actions.First(); 
            return new List<FormField>()
            {
                new FormField()
                {
                    Name = "Name",
                    FieldType = FormFieldTypes.input,
                    Placeholder = $"{this.EntityName} name",
                    AriaInvalid = false,
                    Value = item?.Name,
                },
                FormFieldSelect("State", GetActionStateItems(), GetStateItem(item)),
                FormFieldSelect("Action", Foundations.Operations.Constants.Settings.Actions.ToList(), actionValue),
                new FormField()
                {
                    Name = "Data",
                    FieldType = FormFieldTypes.input,
                    FieldComponent = FormFieldComponents.aclFieldCodeEditor,
                    Hidden = false,
                    Disabled = false,
                    AriaInvalid = false,
                    Value = item?.Data ?? "{}",
                },
                new FormField()
                {
                    Name = "Settings",
                    FieldType = FormFieldTypes.input,
                    FieldComponent = FormFieldComponents.aclFieldCodeEditor,
                    Hidden = false,
                    Disabled = false,
                    AriaInvalid = false,
                    Value = item?.Settings ?? "{}",
                },
                new FormField()
                {
                    Name = "UserId",
                    FieldType = FormFieldTypes.input,
                    Hidden = true,
                    Disabled = true,
                    AriaInvalid = false,
                    Value = user?.Id,
                }
            };
        }

        public AjaxForm EditJobForm(AccountUser user, OperationsActionEntity item)
        {
            var model = base.CreateForm(user, item, PostbackType.PUT);
            model.Label = $"Edit {item.Name}";
            model.Fields = CreateFormFields(user, item);
            return model;
        }
        public override AdminCreatePage CreateEditPage(AccountUser user, IEnumerable<OperationsActionEntity> items, OperationsActionEntity item)
        {
            var viewModel = base.CreateEditPage(user, items, item);
            viewModel.Form = EditJobForm(user, item);
            return viewModel;
        }
        public override AdminIndexPage<OperationsActionEntity> CreateEntityPage(AccountUser user, OperationsActionEntity item, IEnumerable<OperationsActionEntity> items, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var viewModel = base.CreateEntityPage(user, item, items, aggregateResponse);
            viewModel.Form = EditJobForm(user, item);
            viewModel.Form.Disabled = true;
            return viewModel;
        }

    }
}

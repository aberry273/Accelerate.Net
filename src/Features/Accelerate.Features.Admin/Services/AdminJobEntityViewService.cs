using Accelerate.Features.Admin.Models.Views; 
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Common.Models.UI.Components;
using Accelerate.Foundations.Common.Models.UI.Components.Table;
using Accelerate.Foundations.Common.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Operations.Models.Entities;
using Elastic.Clients.Elasticsearch;
using System.Collections.Generic;

namespace Accelerate.Features.Admin.Services
{
    public class AdminJobEntityViewService : AdminBaseEntityViewService<OperationsJobEntity>
    {
        IEntityService<OperationsActionEntity> _actionService;
        IEntityService<OperationsActivityEntity> _activityService;
        public AdminJobEntityViewService(
            IEntityService<OperationsActionEntity> actionService,
            IEntityService<OperationsActivityEntity> activityService,
            IMetaContentService metaContent)
            : base(metaContent)
        {
            _actionService = actionService;
            _activityService = activityService;
            EntityName = "Job";
        }
        public override string GetEntityName(OperationsJobEntity item)
        {
            return item.Name;
        }
        public AjaxForm CreateJobForm(AccountUser user)
        {
            var model = base.CreateForm(user);
            model.Fields = this.CreateFormFields(user, null);
            return model;
        }
        public override AdminCreatePage CreateAddPage(AccountUser user, IEnumerable<OperationsJobEntity> items)
        {
            var viewModel = base.CreateAddPage(user, items);
            viewModel.Form = CreateJobForm(user);
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
        private string GetStateItem(OperationsJobEntity item)
        {
            return Enum.GetName<OperationsJobState>(item?.State ?? OperationsJobState.Published);
        }
        public List<FormField> CreateFormFields(AccountUser user, OperationsJobEntity? item)
        {
            var scheduleValue = item != null
                ? item?.Schedule.ToString()
                : Foundations.Operations.Constants.Settings.Schedules.First().Value;

            var actions = _actionService.Find(x => true);
            var actionItems = actions.Select(x => new KeyValuePair<string, string>(x.Name, x.Id.ToString()));
            var actionValue = item != null
                ? item?.ActionId.ToString()
                : actions.FirstOrDefault()?.Id.ToString();

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
                FormFieldSelect("Schedule", Foundations.Operations.Constants.Settings.Schedules.ToList(), scheduleValue),
                FormFieldSelect("ActionId", "Action", actionItems.ToList(), actionValue),
                new FormField()
                {
                    Name = "UserId",
                    FieldType = FormFieldTypes.input,
                    Hidden = true,
                    Disabled = true,
                    AriaInvalid = false,
                    Value = user.Id,
                }
            };
        }

        public AjaxForm EditJobForm(AccountUser user, OperationsJobEntity item)
        {
            var model = base.CreateEntityForm(user, item, PostbackType.PUT);
            model.Label = $"Edit {item.Name}";
            model.Fields = CreateFormFields(user, item);
            return model;
        }
        public override AdminCreatePage CreateEditPage(AccountUser user, IEnumerable<OperationsJobEntity> items, OperationsJobEntity item)
        {
            var viewModel = base.CreateEditPage(user, items, item);
            viewModel.Form = EditJobForm(user, item);
            return viewModel;
        }
        public override async Task<AdminIndexPage<OperationsJobEntity>> CreateEntityPage(AccountUser user, OperationsJobEntity item, IEnumerable<OperationsJobEntity> items, SearchResponse<ContentPostDocument> aggregateResponse)
        {
            var viewModel = await base.CreateEntityPage(user, item, items, aggregateResponse);
            viewModel.Form = EditJobForm(user, item);
            viewModel.Form.Disabled = true;
            viewModel.Table = this.GetJobActivitiesTable(user, item);
            return viewModel;
        }
        private AclTable<string> GetJobActivitiesTable(AccountUser user, OperationsJobEntity item)
        {
            var model = new AclTable<string>();
            var jobActivities = GetJobActivities(user, item);
            if(jobActivities != null)
            {
                model.Headers = this.GetJobActivitiesHeader(user, item);
                model.Items = jobActivities.Select(CreateJobActivityRowNew).ToList();
            }
            else
            {
                model.Headers = new List<AclTableHeader>();
                model.Items = new List<List<string>>();
            }
            return model;
        }
        private List<string> CreateJobActivityRowNew(OperationsActivityEntity item)
        {
            return new List<string>()
            {
                item.CreatedOn.ToLongDateString(),
                item.Success.ToString(),
                item.Result.ToString()
            };
        }
        private AclTableRow<string> CreateJobActivityRow(OperationsActivityEntity item)
        {
            return new AclTableRow<string>()
            {
                Values = new List<string>()
                {
                    item.CreatedOn.ToLongDateString(),
                    item.Success.ToString(),
                    item.Result.ToString()
                }
            };
        }
        private List<AclTableHeader> GetJobActivitiesHeader(AccountUser user, OperationsJobEntity item)
        {
            return new List<AclTableHeader>()
            {
                new AclTableHeader()
                {
                    Text = "Last ran",
                },
                new AclTableHeader()
                {
                    Text = "Success",
                },
                new AclTableHeader()
                {
                    Text = "Result",
                },
            };
        }
        private IEnumerable<OperationsActivityEntity> GetJobActivities(AccountUser user, OperationsJobEntity item)
        {
            return _activityService.Find(x => x.OperationsJobId == item.Id);
        }
    }
}

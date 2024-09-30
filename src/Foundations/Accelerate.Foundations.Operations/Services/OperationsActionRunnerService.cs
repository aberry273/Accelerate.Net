using Accelerate.Foundations.Common.Models.Data;
using Accelerate.Foundations.Operations.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Operations.Services
{
    public class OperationsActionRunnerService : IOperationsActionRunnerService
    {
        public async Task<OperationResponse<object>> Run(Guid? jobId, Guid actionId, string type, string data, string settings)
        {
            var apiResponse = new OperationResponse<object>()
            {
                Success = true
            };
            var service = new OperationsJobSchedulerService();
            try
            {
                switch (type)
                {
                    case nameof(Foundations.Operations.Constants.Settings.ActionTypes.RssRead):
                        {
                            //Create action object
                            var postbackAction = new RssReaderAction(settings);
                            //Run with data params
                            var result = await postbackAction.RunAsync(data);
                            //Create activity
                            service.CreateJobActivity(jobId, actionId, data, Newtonsoft.Json.JsonConvert.SerializeObject(result.Data), result.Success);
                            apiResponse.Data = result;
                            return apiResponse;
                        }
                    case nameof(Foundations.Operations.Constants.Settings.ActionTypes.Postback):
                        {
                            //Create action object
                            var postbackAction = new PostbackAction(settings);
                            //Run with data params
                            var result = await postbackAction.RunAsync(data);
                            //Create activity
                            service.CreateJobActivity(jobId, actionId, data, Newtonsoft.Json.JsonConvert.SerializeObject(result.Data), result.Success);
                            apiResponse.Data = result;
                            return apiResponse;
                        }
                    case nameof(Foundations.Operations.Constants.Settings.ActionTypes.Email):
                        {
                            //Create action object
                            var postbackAction = new EmailAction(settings);
                            //Run with data params
                            var result = await postbackAction.RunAsync(data);
                            //Create activity
                            service.CreateJobActivity(jobId, actionId, data, Newtonsoft.Json.JsonConvert.SerializeObject(result.Data), result.Success);
                            apiResponse.Data = result;
                            return apiResponse;
                        }
                    case nameof(Foundations.Operations.Constants.Settings.ActionTypes.BulkEmail):
                        {
                            //Create action object
                            var postbackAction = new BulkEmailAction(settings);
                            //Run with data params
                            var result = await postbackAction.RunAsync(data);
                            //Create activity
                            service.CreateJobActivity(jobId, actionId, data, Newtonsoft.Json.JsonConvert.SerializeObject(result.Data), result.Success);
                            apiResponse.Data = result;
                            return apiResponse;
                        }
                        /*
                    case nameof(Foundations.Operations.Constants.Settings.ActionTypes.EmailContacts):
                        {
                            //Create action object
                            var postbackAction = new BulkEmailContactsAction(settings);
                            //Run with data params
                            var result = await postbackAction.RunAsync(data);
                            //Create activity
                            service.CreateJobActivity(organisationId, jobId, actionId, data, Newtonsoft.Json.JsonConvert.SerializeObject(result.Data), result.Success);
                            apiResponse.Data = result;
                            return apiResponse;
                        }
                        */
                    default:
                        {
                            return new OperationResponse<object>()
                            {
                                Success = false,
                                Message = "No Action Function found"
                            };
                        }
                }
            }
            catch (Exception ex)
            {
                service.CreateJobActivity(jobId, actionId, data, ex.Message, false);
                return new OperationResponse<object>()
                {
                    Success = false,
                    Message = ex.ToString()
                };
            }
        }
    }
}

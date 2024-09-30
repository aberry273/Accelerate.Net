
using Accelerate.Foundations.Common.Models.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Operations.Actions
{
    public class CsvImportSettings
    {
        public string Url { get; set; }
        public string Content { get; set; }
        public bool Bulk { get; set; }
        public bool Local { get; set; }
    }
    public class CsvImportAction : BaseAction<CsvImportSettings>
    {
        public CsvImportAction(string settings) : base(settings)
        {
            this.Name = "CsvImportAction";
        }

        public override async Task<OperationResponse<object>> RunAsync(string Data)
        {
            var result = new OperationResponse<object>();
            var csvContent = this.Settings.Content;
            //Parse csv content
            //// ForEach row, do postback action

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(this.Settings.Url))
                {
                    return result;
                }
            }
        }
    }
}

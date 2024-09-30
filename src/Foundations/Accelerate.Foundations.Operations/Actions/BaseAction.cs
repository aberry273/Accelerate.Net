using Accelerate.Foundations.Common.Models.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Accelerate.Foundations.Operations.Actions
{

    public abstract class BaseAction<T> : IAction
    {
        public BaseAction(string settings)
        {
            this.Settings = JsonConvert.DeserializeObject<T>(settings);
        }
        public BaseAction(T settings)
        {
            this.Settings = settings;
        }
        public T Settings { get; set; }
        public string Name { get; set; }
        public string TypeName { get; set; }
        public string Method { get; set; }
        public string Data { get; set; }
        public abstract Task<OperationResponse<object>> RunAsync(string Data);
    }
}

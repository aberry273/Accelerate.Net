
using System.Diagnostics.Contracts;
using System.Runtime.Serialization.DataContracts;

namespace Accelerate.Foundations.EventPipelines.Models.Contracts
{
    public class DataContract<T>
    {
        public required T Data { get; set; }
        public Guid? UserId { get; set; }
        public string Target { get; set; }
    }
    public class CreateDataContract<T> : DataContract<T> { }
    public class CreateCompleteDataContract<T> : DataContract<T> { }
    public class UpdateDataContract<T> : DataContract<T> { }
    public class UpdateCompleteDataContract<T> : DataContract<T> { }
    public class DeleteDataContract<T> : DataContract<T> { }
    public class DeleteCompleteDataContract<T> : DataContract<T> { }
}

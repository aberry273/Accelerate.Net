using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Websockets.Models
{
    public enum DataRequestCompleteType
    {
        Created, Updated, Deleted
    }
    public class WebsocketMessage<T>
    {
        public string Message { get; set; }
        public int Code { get; set; }
        public string Update => Enum.GetName(UpdateType);
        public DataRequestCompleteType UpdateType { get; set; }
        public T Data { get; set; }
        public string Group { get; set; }
    }
}

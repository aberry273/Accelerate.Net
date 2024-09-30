using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Operations.Actions
{
    public interface IAction
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
        public string Method { get; set; }
        public string Data { get; set; }
    }
}

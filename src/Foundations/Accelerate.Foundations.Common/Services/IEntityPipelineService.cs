using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Services
{
    public interface IEntityPipelineService<T, E> where T : IEntityViewModel where E : IBaseEntity
    {
    }
}

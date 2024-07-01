using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.Database.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.EventPipelines.Services
{
    public interface IEntityPipelineService<T, B> : IEntityService<T> where T : BaseEntity
    {
    }
}

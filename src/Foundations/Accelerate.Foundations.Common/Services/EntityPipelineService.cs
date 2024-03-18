using Accelerate.Foundations.Common.Helpers;
using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.Database.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Common.Services
{
    /// <summary>
    /// TODO
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="E"></typeparam>
    public class EntityPipelineService<T, E> : IEntityPipelineService<T, E> where T : IEntityViewModel where E : IBaseEntity
    {
        IEntityService<E> _entityService;
        public EntityPipelineService(IEntityService<E> entityService)
        {
            _entityService = entityService;
        } 

    }
}

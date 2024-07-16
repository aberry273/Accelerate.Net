using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.Services;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.EventPipelines.Controllers
{
    public class BaseApiPipelineController<T, B> : BaseApiController<T> where T : BaseEntity where B : IBus
    {
        new IEntityPipelineService<T, B> _service;

        public BaseApiPipelineController(IEntityPipelineService<T, B> service) : base(service)
        {
            _service = service;
        }
        protected override void UpdateValues(T from, dynamic to)
        {
            throw new NotImplementedException();
        }
    }
}

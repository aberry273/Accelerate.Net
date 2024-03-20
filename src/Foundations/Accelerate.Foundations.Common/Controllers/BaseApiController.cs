using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.Database.Services;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Foundations.Common.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController<T> : ControllerBase where T : IBaseEntity
    {
        protected readonly IEntityService<T> _service;

        public BaseApiController(IEntityService<T> service)
        {
            _service = service;
        }

        [HttpGet]
        public virtual async Task<IActionResult> Get([FromQuery] RequestQuery<T> query)
        {
            int take = query.ItemsPerPage > 0 ? query.ItemsPerPage : 10;
            int skip = take * query.Page;
            return Ok(_service.Find(x => true, skip, take));
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult Get(Guid id)
        {
            var obj = _service.Get(id);
            if (obj == null)
            {
                return NotFound();
            }
            return Ok(obj);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post(T obj)
        {
            var id = await _service.CreateWithGuid(obj);

            if (id == null)
            {
                return BadRequest();
            }

            return Ok(new
            {
                message = "Created Successfully",
                id = id
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] T obj)
        {
            var entity = _service.Get(id);
            if (entity == null)
            {
                return NotFound();
            }
            var hero = await _service.Update(obj);
           

            return Ok(new
            {
                message = "Updated Successfully",
                id = id
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var obj = _service.Get(id);
            if (await _service.Delete(obj) == 1)
            {
                return NotFound();
            }

            return Ok(new
            {
                message = "Deleted Successfully",
                id = id
            });
        }
    }
}
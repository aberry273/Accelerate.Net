using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.Database.Services;
using Accelerate.Projects.App.Models;
using Accelerate.Projects.App.Services;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Projects.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseEntityController<T> : ControllerBase where T : IBaseEntity
    {
        private readonly IEntityService<T> _service;

        public BaseEntityController(IEntityService<T> service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] RequestQuery query)
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
        public async Task<IActionResult> Post(T obj)
        {
            var id = await _service.CreateWithGuid(obj);

            if (id == null)
            {
                return BadRequest();
            }

            return Ok(new
            {
                message = "Super Hero Created Successfully!!!",
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
                message = "Super Hero Updated Successfully!!!",
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
                message = "Super Hero Deleted Successfully!!!",
                id = id
            });
        }
    }
}
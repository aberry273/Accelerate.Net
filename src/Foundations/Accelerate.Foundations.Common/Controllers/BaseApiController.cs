using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.Database.Services;
using Azure;
using Accelerate.Foundations.Common.Models.UI.Components;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        [HttpPost("query")]
        public virtual async Task<IActionResult> Query([FromBody] QueryRequestModel<T> request)
        {
            try
            {
                int take = request.ItemsPerPage > 0 ? request.ItemsPerPage ?? 10 : 10;
                if (take > 100) take = 100;
                int skip = take * request.CurrentPage;
                return Ok(_service.Find(x => true, skip, take));
            }
            catch (Exception ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
                return BadRequest();
            }
        }

        [HttpGet]
        public virtual async Task<IActionResult> Get([FromQuery] int Page = 0, [FromQuery] int ItemsPerPage = 10, [FromQuery] string? Text = null)
        {
            try
            {
                int take = ItemsPerPage > 0 ? ItemsPerPage : 10;
                if (take > 100) take = 100;
                int skip = take * Page;
                return Ok(_service.Find(x => true, skip, take));
            }
            catch (Exception ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("{id}")]
        public virtual IActionResult Get(Guid id)
        {
            try
            {
                var obj = _service.Get(id);
                if (obj == null)
                {
                    return NotFound();
                }
                return Ok(obj);
            }
            catch (Exception ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
                return BadRequest();
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post(T obj)
        {
            try
            {
                var id = await _service.CreateWithGuid(obj);

                if (id == null)
                {
                    return ValidationProblem();
                }
                //To override
                var entity = _service.Get(id.GetValueOrDefault());
                await PostCreateSteps(entity);

                return Ok(new
                {
                    message = "Created Successfully",
                    id = id
                });
            }
            catch (Exception ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
                return BadRequest();
            }
        }
        protected virtual async Task PostCreateSteps(T obj)
        {
            await Task.Run(() => { return; });
        }

        protected abstract void UpdateValues(T from, dynamic to);

        [HttpPut]
        [Route("{id}")]
        public virtual async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] T obj)
        {
            try
            {
                var entity = _service.Get(id);
                if (entity == null)
                {
                    return NotFound();
                }
                UpdateValues(entity, obj);
                entity.UpdatedOn = DateTime.Now;
                await _service.Update(entity);
                //To override
                var updatedEntity = _service.Get(id);
                await PostUpdateSteps(updatedEntity);


                return Ok(new
                {
                    message = "Updated Successfully",
                    id = id
                });
            }
            catch (Exception ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
                return BadRequest();
            }
        }
        protected virtual async Task PostUpdateSteps(T obj)
        {
            await Task.Run(() => { return; });
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                var obj = _service.Get(id);
                if(obj == null)
                {
                    return NotFound();
                }
                var entity = _service.Get(id);
                var result = await _service.Delete(obj);
                await PostDeleteSteps(entity);
                return Ok(new
                {
                    message = "Deleted Successfully",
                    id = id
                });
            }
            catch (Exception ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
                return StatusCode(500, "There was an error processing your request, please try again.");
            }
        }

        protected virtual async Task PostDeleteSteps(T obj)
        {
            await Task.Run(() => { return; });
        }
    }
}
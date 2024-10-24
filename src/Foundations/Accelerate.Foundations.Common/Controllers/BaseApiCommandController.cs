using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.Database.Services;
using Azure;
using Accelerate.Foundations.Common.Models.UI.Components;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using MediatR;
using Accelerate.Foundations.Mediator.Queries;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Accelerate.Foundations.Mediator.Commands;

namespace Accelerate.Foundations.Common.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiCommandController<T> : ControllerBase where T : IBaseEntity
    {
        private readonly IMediator _mediator;

        public BaseApiCommandController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost("query")]
        public virtual async Task<IActionResult> Query([FromBody] QueryRequestModel<T> request)
        {
            var response = await _mediator.Send(new FindEntityQuery<T>());
            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual async Task<IActionResult> Get(Guid id)
        {
            var response = await _mediator.Send(new GetIdEntityQuery<T>() { Id = id });
            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post(T entity)
        {
            if (entity is null) return BadRequest();

            var command = new CreateEntityCommand<T>() { Entity = entity };
            var response = await _mediator.Send(command);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] T entity)
        {
            if (entity is null) return BadRequest();

            var command = new UpdateEntityCommand<T>() { Entity = entity };

            var response = await _mediator.Send(command);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        } 

        [HttpDelete]
        [Route("{id}")]
        public virtual async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            if (id == Guid.Empty) return BadRequest();

            var command = new DeleteEntityCommand<T>() { Id = id };

            var response = await _mediator.Send(command);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        protected virtual async Task PostDeleteSteps(T obj)
        {
            await Task.Run(() => { return; });
        }
    }
}
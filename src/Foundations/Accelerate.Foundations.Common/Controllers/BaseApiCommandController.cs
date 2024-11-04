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
using Microsoft.AspNetCore.Http;
using System;

namespace Accelerate.Foundations.Common.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public abstract class BaseApiCommandController<T> : ControllerBase where T : IBaseEntity
    {
        private readonly IMediator _mediator;

        public BaseApiCommandController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        ///  Use this API to query and return an array of items. Pass in the properties you want to search by using the Query property. The Query object is required with at least one property sent through of the item.
        /// </summary>
        /// <param name="Query">The query object to represent the item you are quering. If searching for a specific object you can pass through the object ID</param>
        /// <param name="Page">Defines the page number to return items from, depending on the items per page.</param>
        /// <param name="PageSize">Defines how many items in a page you want to return.</param>
        /// <returns>A paginated list of items that match the query objects properties.</returns>
        /// <remarks>
        /// Sample request:
        ///     POST /query
        ///     {
        ///        "query": {
        ///             "Name": "Example"
        ///        },
        ///        page: 3,
        ///        pageSize: 5 
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Returns the list of items matching the query properties and page parameters</response>
        /// <response code="400">If no items could be found</response>
        [HttpPost("query")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public virtual async Task<IActionResult> Query([FromBody] QueryRequestModel<T> request)
        {
            var response = await _mediator.Send(new FindEntityQuery<T>());
            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        /// <summary>
        ///  Use this API to return a single item. 
        /// </summary>
        /// <param name="Id">The ID of the item you want to retrieve</param>
        /// <returns>A paginated list of items that match the query objects properties.</returns>
        /// <remarks>
        /// Sample request:
        ///     GET /query/{id}
        ///
        /// </remarks>
        /// <response code="201">Returns the item matching the ID sent through in the request parameter</response>
        /// <response code="400">If no item could be found that matches the ID</response>
        [HttpGet]
        [Route("{id}")]
        public virtual async Task<IActionResult> Get(string Id)
        {
            Guid guid;
            var parsed = Guid.TryParse(Id, out guid);
            if(!parsed)
            {
                return BadRequest("Invalid GUID");
            }

            var response = await _mediator.Send(new GetIdEntityQuery<T>() { Id = guid });
            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        /// <summary>
        ///  Use this API to query and return an array of items. Pass in the properties you want to search by using the Query property. The Query object is required with at least one property sent through of the item.
        /// </summary>
        /// <param name="Query">The query object to represent the item you are quering. If searching for a specific object you can pass through the object ID</param>
        /// <param name="Page">Defines the page number to return items from, depending on the items per page.</param>
        /// <param name="PageSize">Defines how many items in a page you want to return.</param>
        /// <returns>A paginated list of items that match the query objects properties.</returns>
        /// <remarks>
        /// Sample request:
        ///     POST /
        ///     {
        ///        {
        ///             "Name": "Example",
        ///             ...
        ///        },
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Returns the ID of the item created</response>
        /// <response code="400">If an item could not be created</response>
        [HttpPost]
        public virtual async Task<IActionResult> Post(T entity)
        {
            if (entity is null) return BadRequest();
            
            Guid guid = Guid.NewGuid();
            entity.Id = guid;
            var command = new CreateEntityCommand<T>() { Entity = entity };
            var response = await _mediator.Send(command);

            if (response.Success)
            {
                var getResponse = await _mediator.Send(new GetIdEntityQuery<T>() { Id = guid });
                
                var item = getResponse.Success ?
                    getResponse.Data
                    : entity;

                var data = new ActionResult<T>(item);

                return Ok(response);
            }

            return BadRequest(response);
        }

        /// <summary>
        ///  Use this API to update an item. Pass in the properties you want to update in the body of the document and the ID as a route parameter.
        /// </summary>
        /// <param name="Id">The ID of the item you want to update</param>
        /// <param name="Query">The query object to represent the item you are quering. If searching for a specific object you can pass through the object ID</param>
        /// <returns>A paginated list of items that match the query objects properties.</returns>
        /// <remarks>
        /// Sample request:
        ///     PUT /{id}
        ///     {
        ///        {
        ///             "Name": "Example",
        ///             ...
        ///        },
        ///     }
        ///
        /// </remarks>
        /// <response code="201">Returns the ID of the item updated</response>
        /// <response code="400">If the item could not be updated</response>
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

        /// <summary>
        ///  Use this API to delete an item. Pass in the ID of the item you want to delete as a route parameter.
        /// </summary>
        /// <param name="Id">The ID of the item you want to delete</param>
        /// <returns>A paginated list of items that match the query objects properties.</returns>
        /// <remarks>
        /// Sample request:
        ///     DELETE /{id}
        ///
        /// </remarks>
        /// <response code="201">Returns the ID of the item deleted</response>
        /// <response code="400">If the item could not be deleted</response>
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
    }
}
using Accelerate.Foundations.Common.Controllers;
using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.Database.Services;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Features.Content.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentPostController : BaseApiController<ContentPostEntity>
    { 
        public ContentPostController(IEntityService<ContentPostEntity> service) : base(service)
        {

        }
    }
}
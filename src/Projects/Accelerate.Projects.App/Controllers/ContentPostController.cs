using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.Database.Services;
using Accelerate.Projects.App.Models;
using Accelerate.Projects.App.Services;
using Microsoft.AspNetCore.Mvc;

namespace Accelerate.Projects.App.Controllers
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
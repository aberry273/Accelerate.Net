using Accelerate.Features.Content.Models.Data;
using Accelerate.Foundations.Content.Models;

namespace Accelerate.Features.Content.Services
{
    public interface IContentPostElasticService
    {
        Task<ContentPost> Index(ContentPostEntity entity);
    }
}
